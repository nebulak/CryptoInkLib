using Org.BouncyCastle.Bcpg;
using Org.BouncyCastle.Bcpg.OpenPgp;
using Org.BouncyCastle.Bcpg.Sig;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using System;
using System.Collections;
using System.IO;

namespace CryptoInkLib
{
	public class OpenPgpCrypter
	{
		public OpenPgpCrypter ()
		{
		}
		//src: https://gist.github.com/dieseltravis/8323431

		private PgpPublicKeyRing m_PublicKeyRing;
		private PgpSecretKeyRing m_PrivateKeyRing;
		private char[] m_cPassword;


		public OpenPgpCrypter(PgpPublicKeyRing publicKeyRing, PgpSecretKeyRing privateKeyRing, char[] cPassword)
		{
			if ( publicKeyRing == null || privateKeyRing == null || cPassword == null)
				throw new ArgumentNullException("encryptionKeys", "encryptionKeys or password is null.");

			this.m_PublicKeyRing = publicKeyRing;
			this.m_PrivateKeyRing = privateKeyRing;
			this.m_cPassword = cPassword;
		}



		public string encryptPgpString(string inputString, long receiverId)
		{
			// use armor: yes, use integrity check? yes?
			return encryptPgpString(inputString, receiverId, true, true);
		}


		public string encryptPgpString(string inputString, string sReceiver, bool armor, bool withIntegrityCheck)
		{
			PgpPublicKey pubKey = ReadPublicKey(sReceiver);

			using (MemoryStream outputBytes = getMemoryStreamFromString(inputString))
			{
				PgpEncryptedDataGenerator dataGenerator = new PgpEncryptedDataGenerator(SymmetricKeyAlgorithmTag.Aes256, withIntegrityCheck, new SecureRandom());

				dataGenerator.AddMethod(pubKey);
				byte[] dataBytes = outputBytes.ToArray();

				using ( Stream outputStream = new MemoryStream() )
				{
					if (armor)
					{
						using (ArmoredOutputStream armoredStream = new ArmoredOutputStream(outputStream))
						{
							Stream encryptedStream = writeStream(dataGenerator.Open(armoredStream, dataBytes.Length), ref dataBytes);
							string sOutputString = getStringFromStream (encryptedStream);
							return sOutputString;

						}
					}
					else
					{
						Stream encryptedStream = writeStream(dataGenerator.Open(outputStream, dataBytes.Length), ref dataBytes);
						string sOutputString = getStringFromStream (encryptedStream);
						return sOutputString;
					}
				}
			}
		}

		public string encryptPgpString(string inputString, long receiverId, bool armor, bool withIntegrityCheck)
		{
			PgpPublicKey pubKey = ReadPublicKey(receiverId);

			using (MemoryStream outputBytes = getMemoryStreamFromString(inputString))
			{
				PgpEncryptedDataGenerator dataGenerator = new PgpEncryptedDataGenerator(SymmetricKeyAlgorithmTag.Aes256, withIntegrityCheck, new SecureRandom());

				dataGenerator.AddMethod(pubKey);
				byte[] dataBytes = outputBytes.ToArray();

				using ( Stream outputStream = new MemoryStream() )
				{
					if (armor)
					{
						using (ArmoredOutputStream armoredStream = new ArmoredOutputStream(outputStream))
						{
							Stream encryptedStream = writeStream(dataGenerator.Open(armoredStream, dataBytes.Length), ref dataBytes);
							string sOutputString = getStringFromStream (encryptedStream);
							return sOutputString;

						}
					}
					else
					{
						Stream encryptedStream = writeStream(dataGenerator.Open(outputStream, dataBytes.Length), ref dataBytes);
						string sOutputString = getStringFromStream (encryptedStream);
						return sOutputString;
					}
				}
			}
		}


		public string DecryptPgpString(string sInput)
		{
			string sOutput;
			using (MemoryStream inputStream = this.getMemoryStreamFromString(sInput))
			{
				sOutput = DecryptPgpString (inputStream);
			}
			return sOutput;
		}


		public string DecryptPgpString(Stream inputStream)
		{
			string sOutput;

			PgpObjectFactory pgpFactory = new PgpObjectFactory(PgpUtilities.GetDecoderStream(inputStream));

			// find secret key
			PgpObject pgp = null;
			if (pgpFactory != null)
			{
				pgp = pgpFactory.NextPgpObject();
			}

			// the first object might be a PGP marker packet.
			PgpEncryptedDataList encryptedData = null;
			if (pgp is PgpEncryptedDataList)
			{
				encryptedData = (PgpEncryptedDataList)pgp;
			}
			else
			{
				encryptedData = (PgpEncryptedDataList)pgpFactory.NextPgpObject();
			}

			// decrypt
			PgpPrivateKey privateKey = null;
			PgpPublicKeyEncryptedData pubKeyData = null;
			foreach (PgpPublicKeyEncryptedData pubKeyDataItem in encryptedData.GetEncryptedDataObjects())
			{
				privateKey = FindSecretKey(pubKeyDataItem.KeyId);

				if (privateKey != null)
				{
					pubKeyData = pubKeyDataItem;
					break;
				}
			}

			if (privateKey == null)
			{
				throw new ArgumentException("Secret key for message not found.");
			}

			PgpObjectFactory plainFact = null;
			using (Stream clear = pubKeyData.GetDataStream(privateKey))
			{
				plainFact = new PgpObjectFactory(clear);
			}

			PgpObject message = plainFact.NextPgpObject();

			if (message is PgpCompressedData)
			{
				PgpCompressedData compressedData = (PgpCompressedData)message;
				PgpObjectFactory pgpCompressedFactory = null;

				using (Stream compDataIn = compressedData.GetDataStream())
				{
					pgpCompressedFactory = new PgpObjectFactory(compDataIn);
				}

				message = pgpCompressedFactory.NextPgpObject();
				PgpLiteralData literalData = null;
				if (message is PgpOnePassSignatureList)
				{
					message = pgpCompressedFactory.NextPgpObject();
				}

				literalData = (PgpLiteralData)message;
				using (Stream unc = literalData.GetInputStream())
				{
					sOutput = this.getStringFromStream(unc);
				}

			}
			else if (message is PgpLiteralData)
			{
				PgpLiteralData literalData = (PgpLiteralData)message;
				using (Stream unc = literalData.GetInputStream())
				{
					sOutput = this.getStringFromStream(unc);
				}
			}
			else if (message is PgpOnePassSignatureList)
			{
				throw new PgpException("Encrypted message contains a signed message - not literal data.");
			}
			else
			{
				throw new PgpException("Message is not a simple encrypted file - type unknown.");
			}

			return sOutput;
		}



		// private helper functions

		private PgpPublicKey ReadPublicKey(long receiverId)
		{
			foreach (PgpPublicKey key in this.m_PublicKeyRing.GetPublicKeys())
			{
				if (key.IsEncryptionKey && key.KeyId == receiverId)
				{
					return key;
				}
			}

			throw new ArgumentException("Can't find encryption key in key ring.");
		}

		public PgpPublicKey ReadPublicKey(string sEmail)
		{
			foreach (PgpPublicKey publicKey in m_PublicKeyRing.GetPublicKeys()) {
				//check if emailaddress is correct
				foreach (object userId in publicKey.GetUserIds()) {
					//Prints "My_Key_Name (Notes) <my_email@gmail.com>"
					if (userId.ToString ().Contains ("@")) {
						char[] separators = { '<', '>' };
						string[] sSubStrings = userId.ToString ().Split (separators);

						if (sSubStrings.Length > 1) {
							for (int i = 0; i < sSubStrings.Length; i++) {
								if (sSubStrings [i].ToLower () == sEmail.ToLower ()) {
									//check if key is encryption key
									if (publicKey.IsEncryptionKey)
									{
										return publicKey;
									}
								}
							}
						}
					}
				}
			}
			throw new ArgumentException("Can't find encryption key in key ring.");
		}

		private PgpPrivateKey FindSecretKey(long keyId)
		{
			PgpSecretKey pgpPrivateKey = this.m_PrivateKeyRing.GetSecretKey (keyId);
			if (pgpPrivateKey == null)
			{
				return null;
			}

			return pgpPrivateKey.ExtractPrivateKey(this.m_cPassword);
		}


		private Stream writeStream(Stream inputStream, ref byte[] dataBytes)
		{
			using (Stream outputStream = inputStream)
			{
				outputStream.Write(dataBytes, 0, dataBytes.Length);
				return outputStream;
			}
		}

		private string getStringFromStream(Stream inputStream)
		{
			string output;
			using (StreamReader reader = new StreamReader(inputStream))
			{
				output = reader.ReadToEnd();
			}
			return output;
		}

		private MemoryStream getMemoryStreamFromString(string sInput)
		{
			MemoryStream stream = new MemoryStream();
			using (StreamWriter writer = new StreamWriter (stream)) {
				writer.Write (sInput);
				writer.Flush ();
			}
			stream.Position = 0;
			return stream;
		}

	}
}


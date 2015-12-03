using System;
using System.IO;
using System.Text;
using System.Reflection;
using Newtonsoft.Json;
using Org.BouncyCastle.Crypto; //PBEParametersGenerator;
using Org.BouncyCastle.Crypto.Generators; //PKCS5S2ParametersGenerator;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.X509;


using Org.BouncyCastle.Crypto.Digests; //SHA3Digest;
using Org.BouncyCastle.Crypto.Parameters; //.KeyParameter;
using Org.BouncyCastle.Crypto.Prng; //DigestRandomGenerator;


//for aes encryption
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Paddings;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.Crypto.IO;
using Org.BouncyCastle.Utilities.IO.Pem;

//for HMAC
using Org.BouncyCastle.Crypto.Macs;
using Org.BouncyCastle.Math;

namespace CryptoInkLib
{
	public class FileCrypter
	{


		public FileCrypter ()
		{

		}

		public int encryptFile(FriendUserKey[] baRecipientKeys, Stream streamPlainFile, string sPath)
		{
			//create Random Number Generator
			var rng = new System.Security.Cryptography.RNGCryptoServiceProvider();

			//create passwordKey from password
			byte[] salt = new byte[16];
			rng.GetBytes (salt);

			//create needed keys
			byte [] baContentIv = new byte[16];
			byte [] baContentKey1 = new byte[16];
			byte [] baContentKey2 = new byte[16];
			byte[] baContentKey = new byte[32];
			byte [] baFilenameIv = new byte[16];
			byte [] baFilenameKey = new byte[16];

			rng.GetBytes(baContentIv);
			rng.GetBytes(baContentKey1);
			rng.GetBytes(baContentKey2);
			rng.GetBytes(baFilenameIv);
			rng.GetBytes(baFilenameKey);

			baContentKey1.CopyTo (baContentKey, 0);
			baContentKey2.CopyTo (baContentKey, baContentKey1.Length);



			//create FileHeader
			//TODO: get encrypted filename
			string sEncryptedFilename = FileNameCrypter.encryptFilename(Path.GetFileName(sPath), baFilenameKey, baFilenameKey);

			string sFileHeader = FileHeaderCrypter.createFileHeader(baContentIv, baContentKey, baFilenameIv, baFilenameKey, baRecipientKeys);
			File.WriteAllText(sPath + "\\" + sEncryptedFilename, sFileHeader);

			using (Stream encryptedContent = FileContentCrypter.encryptStream (baContentKey1, baContentKey2, streamPlainFile)) {
				using (FileStream fsOutputFile = new FileStream(sPath + "\\" + sEncryptedFilename, FileMode.Append, FileAccess.Write)) {
					encryptedContent.CopyTo (fsOutputFile);
				}

			}

			return 0;
		}


		public int decryptFile(UserKey userKey, Stream streamCipherFile, string sPath)
		{
			byte[] baFileHeader = new byte [streamCipherFile.Length];
			string sFileHeader = "";
			//get fileheader as string
			for (int i = 0; i < streamCipherFile.Length; i++) {
				streamCipherFile.Read(baFileHeader, i, 1);

				byte[] baCurrentChar = new byte[1];
				baCurrentChar [0] = baFileHeader [i];
				sFileHeader += Encoding.UTF8.GetString (baCurrentChar);

				//check for end of fileheader
				if (i > 3) 
				{
					if (sFileHeader.Contains (FileHeaderCrypter.HEADER_END)) 
					{
						CInkPlainHeader fileHeader = FileHeaderCrypter.getDecryptedHeader (sFileHeader, userKey);
						byte[] contentKey = fileHeader.content_keys [0].sessionKey;
						byte[] key1 = new byte[16];
						byte[] key2 = new byte[16];

						//get AES-XTS-Keys
						for (int j = 0; j < 32; j++) 
						{
							if (i < 16) {
								key1 [i] = contentKey [i];
							} else {
								key2 [i] = contentKey [i];
							}
						}

						string sFilename = FileNameCrypter.decryptFilename (Path.GetFileName (sPath), fileHeader.filename_keys[0].sessionKey, fileHeader.filename_iv );


						using (Stream plain_content = FileContentCrypter.decryptStream (key1, key2, streamCipherFile)) {
							using (FileStream fsOutputFile = new FileStream (sPath + "\\" + sFilename, FileMode.Create, FileAccess.Write)) {
								plain_content.CopyTo (fsOutputFile);
								return 0;
							}
						}
					}
				}

			}
			return 1;

		}

		public static Stream getCryptoStream(UserKey userKey, Stream streamCipherFile, string sPath)
		{
			byte[] baFileHeader = new byte [streamCipherFile.Length];
			string sFileHeader = "";
			//get fileheader as string
			for (int i = 0; i < streamCipherFile.Length; i++) {
				streamCipherFile.Read(baFileHeader, i, 1);

				byte[] baCurrentChar = new byte[1];
				baCurrentChar [0] = baFileHeader [i];
				sFileHeader += Encoding.UTF8.GetString (baCurrentChar);

				//check for end of fileheader
				if (i > 3) 
				{
					if (sFileHeader.Contains (FileHeaderCrypter.HEADER_END)) 
					{
						CInkPlainHeader fileHeader = FileHeaderCrypter.getDecryptedHeader (sFileHeader, userKey);
						byte[] contentKey = fileHeader.content_keys [0].sessionKey;
						byte[] key1 = new byte[16];
						byte[] key2 = new byte[16];

						//get AES-XTS-Keys
						for (int j = 0; j < 32; j++) 
						{
							if (i < 16) {
								key1 [i] = contentKey [i];
							} else {
								key2 [i] = contentKey [i];
							}
						}

						string sFilename = FileNameCrypter.decryptFilename (Path.GetFileName (sPath), fileHeader.filename_keys[0].sessionKey, fileHeader.filename_iv );


						return FileContentCrypter.decryptStream (key1, key2, streamCipherFile);
					}
				}
			}
			return null;
		}
	}
}


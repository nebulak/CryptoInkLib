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

namespace CryptoInkLib
{
	public class FileNameCrypter
	{
		public FileNameCrypter ()
		{
		}

		//TODO:
		public static string encryptFilename(string sPlainFileName, byte [] baFileNameKey, byte[] baFilenameIv)
		{
			const int MacBitSize = 128;
			byte [] baPayload = new byte[0];

			var encryptCipher = new GcmBlockCipher(new AesFastEngine());
			var parameters = new AeadParameters(new KeyParameter(baFileNameKey), MacBitSize, baFilenameIv, baPayload);
			encryptCipher.Init (true, parameters);

			byte[] baPlainFileName = Encoding.UTF8.GetBytes(sPlainFileName);// get Bytes from ASCII-String

			var cipherText = new byte[encryptCipher.GetOutputSize(baPlainFileName.Length)];
			var len = encryptCipher.ProcessBytes(baPlainFileName, 0, baPlainFileName.Length, cipherText, 0);
			encryptCipher.DoFinal(cipherText, len);

			//TODO: change representation to Hex
			string sCipherString = Convert.ToBase64String (cipherText);
			return sCipherString;
		}


		public static string decryptFilename(string sCipherFileName, byte [] baFileNameKey, byte[] baFileNameIv)
		{
			const int MacBitSize = 128;
			byte [] baPayload = new byte[0];

			var decryptCipher = new GcmBlockCipher(new AesFastEngine());
			var parameters = new AeadParameters(new KeyParameter(baFileNameKey), MacBitSize, baFileNameIv, baPayload);
			decryptCipher.Init (false, parameters);

			//TODO: change to Hex
			byte[] baCipherFileName = Convert.FromBase64String (sCipherFileName);

			var decryptedText = new byte[decryptCipher.GetOutputSize(baCipherFileName.Length)];
			try
			{
				var len = decryptCipher.ProcessBytes(baCipherFileName, 0, baCipherFileName.Length, decryptedText, 0);
				decryptCipher.DoFinal(decryptedText, len);
			}
			catch (InvalidCipherTextException)
			{
				//Return null if it doesn't authenticate
				return null;
			} 
				
			string sPlainfileName = Encoding.UTF8.GetString (decryptedText);

			return sPlainfileName;
		}
	}
}


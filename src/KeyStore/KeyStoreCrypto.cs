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
	public class KeyStoreCrypto
	{
		public KeyStoreCrypto ()
		{
		}

		public static AsymmetricCipherKeyPair generateUserKeyPair(int c_iKeySizeInBits)
		{
			//generate RSA-Keypair for file encryption
			RsaKeyPairGenerator _RSAKeyPairGenerator = new RsaKeyPairGenerator();
			_RSAKeyPairGenerator.Init(new KeyGenerationParameters(new SecureRandom(), c_iKeySizeInBits));
			var userKey = _RSAKeyPairGenerator.GenerateKeyPair();

			return userKey;
		}



		public static byte[] encodeUserKeyPairPrivate(AsymmetricCipherKeyPair userKey)
		{
			PrivateKeyInfo privateKeyInfo = PrivateKeyInfoFactory.CreatePrivateKeyInfo(userKey.Private);
			byte[] serializedPrivateBytes = privateKeyInfo.ToAsn1Object().GetDerEncoded();

			return serializedPrivateBytes;
		}



		private static RsaPrivateCrtKeyParameters decodeUserKeyPairPrivate(String privateKeyString)
		{
			RsaPrivateCrtKeyParameters privateKey = (RsaPrivateCrtKeyParameters) PrivateKeyFactory.CreateKey(Convert.FromBase64String(privateKeyString));
			return privateKey;
		}



		public static RsaKeyParameters decodeUserKeyPairPublic(String publicKeyString)
		{
			RsaKeyParameters publicKey = (RsaKeyParameters) PublicKeyFactory.CreateKey(Convert.FromBase64String(publicKeyString));
			return publicKey;
		}



		public static byte[] encodeUserKeyPairPublic(AsymmetricCipherKeyPair userKey)
		{
			SubjectPublicKeyInfo publicKeyInfo = SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(userKey.Public);
			byte[] serializedPublicBytes = publicKeyInfo.ToAsn1Object().GetDerEncoded();

			return serializedPublicBytes;
		}



		public static byte[] createPasswordKey(string password, byte[] salt)
		{
			//TODO: change hash algorithm...
			var iterations = 10000; // Choose a value that will perform well given your hardware.
			var pbkdf2 = new System.Security.Cryptography.Rfc2898DeriveBytes(password, salt, iterations);
			var hash = pbkdf2.GetBytes(32); // Get 32 bytes for the hash

			return hash;
		}



		public static bool isPasswordValid(string c_sPassword, byte [] c_baSalt, byte [] c_baPasswordKey)
		{
			//TODO: change hash algorithm...
			var iterations = 10000; // Choose a value that will perform well given your hardware.
			var pbkdf2 = new System.Security.Cryptography.Rfc2898DeriveBytes(c_sPassword, c_baSalt, iterations);
			var hash = pbkdf2.GetBytes(32); // Get 32 bytes for the hash

			if (hash == c_baPasswordKey) 
			{
				return true;
			}

			return false;
		}



		public static string encryptKeyStoreStorage(byte[] c_baKey, byte[] c_baIV, KeyStoreStorage c_KeyStoreStorage)
		{
			const int MacBitSize = 128;
			byte [] baPayload = new byte[0];

			var encryptCipher = new GcmBlockCipher(new AesFastEngine());
			var parameters = new AeadParameters(new KeyParameter(c_baKey), MacBitSize, c_baIV, baPayload);
			encryptCipher.Init (true, parameters);

			string sJsonStorage = JsonConvert.SerializeObject (c_KeyStoreStorage);//ConvertToJSON and
			byte[] baJsonStorage = Encoding.GetEncoding(1252).GetBytes(sJsonStorage);// get Bytes from ASCII-String

			var cipherText = new byte[encryptCipher.GetOutputSize(baJsonStorage.Length)];
			var len = encryptCipher.ProcessBytes(baJsonStorage, 0, baJsonStorage.Length, cipherText, 0);
			encryptCipher.DoFinal(cipherText, len);

			string sCipherString = Convert.ToBase64String (cipherText);
			return sCipherString;
		}



		public static KeyStoreStorage decryptKeyStoreStorage(byte[] c_baKey, byte[] c_baIV, string c_sBase64JsonStorage)
		{
			const int MacBitSize = 128;
			byte [] baPayload = new byte[0];

			var decryptCipher = new GcmBlockCipher(new AesFastEngine());
			var parameters = new AeadParameters(new KeyParameter(c_baKey), MacBitSize, c_baIV, baPayload);
			decryptCipher.Init (false, parameters);

			byte[] baEncryptedStorage = Convert.FromBase64String (c_sBase64JsonStorage);

			var decryptedText = new byte[decryptCipher.GetOutputSize(baEncryptedStorage.Length)];
			try
			{
				var len = decryptCipher.ProcessBytes(baEncryptedStorage, 0, baEncryptedStorage.Length, decryptedText, 0);
				decryptCipher.DoFinal(decryptedText, len);
			}
			catch (InvalidCipherTextException)
			{
				//Return null if it doesn't authenticate
				return null;
			} 

			string sJsonStorage = Encoding.GetEncoding (1252).GetString (decryptedText);
			KeyStoreStorage _KeyStoreStorage = JsonConvert.DeserializeObject <KeyStoreStorage> (sJsonStorage);

			return _KeyStoreStorage;
		}
	}
}


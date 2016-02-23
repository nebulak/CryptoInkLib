using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;
using System.Reflection;
using Newtonsoft.Json;
using Org.BouncyCastle.Crypto; //PBEParametersGenerator;
using Org.BouncyCastle.Crypto.Generators; //PKCS5S2ParametersGenerator;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.X509;
using Org.BouncyCastle.Crypto.Prng;


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
	public class CommonCrypto
	{
		public CommonCrypto ()
		{
		}
		//TODO:
		//put static crypto functions in this file e.g.
		//RsaEncrypt
		//decodeUserKey
		//encodeUserKey
		//AES XTS-Encrypt
		//AES XTS-Decrypt

		public static byte[] RsaEncrypt(string b64_public_key, byte[] plain_text)
		{
			RsaKeyParameters publicKey = decodeUserKeyPairPublic (b64_public_key);

			// Creating the RSA algorithm object
			IAsymmetricBlockCipher cipher = new RsaEngine();

			// Initializing the RSA object for Encryption with RSA public key. Remember, for encryption, public key is needed
			cipher.Init(true, publicKey);

			//Encrypting the input bytes
			byte[] encryptedBytes = cipher.ProcessBlock(plain_text, 0, plain_text.Length);

			return encryptedBytes;
		}

		public static byte[] RsaDecrypt(string b64_private_key, byte [] cipher_text)
		{
			// Extracting the private key from the pair
			RsaKeyParameters privateKey = decodeUserKeyPairPrivate(b64_private_key);

			// Creating the RSA algorithm object
			IAsymmetricBlockCipher cipher = new RsaEngine();
			cipher.Init(false, privateKey);
			byte[] plain_text = cipher.ProcessBlock(cipher_text, 0, cipher_text.Length);

			return plain_text;
		}


		public static byte[] encodeUserKeyPairPrivate(AsymmetricCipherKeyPair userKey)
		{
			PrivateKeyInfo privateKeyInfo = PrivateKeyInfoFactory.CreatePrivateKeyInfo(userKey.Private);
			byte[] serializedPrivateBytes = privateKeyInfo.ToAsn1Object().GetDerEncoded();

			return serializedPrivateBytes;
		}



		public static RsaPrivateCrtKeyParameters decodeUserKeyPairPrivate(String privateKeyString)
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


		//src: http://stackoverflow.com/questions/1344221/how-can-i-generate-random-alphanumeric-strings-in-c
		public static string getRandomString(int maxSize)
		{
			char[] chars = new char[62];
			chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();
			byte[] data = new byte[1];
			//TODO: use own PRNG !
			using (RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider())
			{
				crypto.GetNonZeroBytes(data);
				data = new byte[maxSize];
				crypto.GetNonZeroBytes(data);
			}
			StringBuilder result = new StringBuilder(maxSize);
			foreach (byte b in data)
			{
				result.Append(chars[b % (chars.Length)]);
			}
			return result.ToString();
		}
	}
}


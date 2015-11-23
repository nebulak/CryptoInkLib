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
			RsaKeyParameters publicKey = KeyStoreCrypto.decodeUserKeyPairPublic (b64_public_key);

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
			RsaKeyParameters privateKey = KeyStoreCrypto.decodeUserKeyPairPrivate(b64_private_key);

			// Creating the RSA algorithm object
			IAsymmetricBlockCipher cipher = new RsaEngine();
			cipher.Init(false, privateKey);
			byte[] plain_text = cipher.ProcessBlock(cipher_text, 0, cipher_text.Length);

			return plain_text;
		}
	}
}


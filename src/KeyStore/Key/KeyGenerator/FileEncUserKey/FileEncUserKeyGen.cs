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
	public class FileEncUserKeyGen
	{
		public FileEncUserKeyGen ()
		{
		}

		public static FileEncUserKey generate()
		{
			//create and encode userKey
			var userKeyPair 		= generateUserKeyPair (4096);
			var encodedPrivate 		= encodeUserKeyPairPrivate (userKeyPair);
			var serializedPublic 	= encodeUserKeyPairPublic (userKeyPair);

			FileEncUserKey fileEncKey = new FileEncUserKey ();
			fileEncKey.publicKey = Convert.ToBase64String (serializedPublic);
			fileEncKey.privateKey = Convert.ToBase64String (encodedPrivate);
			fileEncKey.status = 1;
			fileEncKey.creationDate = DateTime.Now.ToString("d/M/yyyy");
			fileEncKey.revocationDate = null;

			return fileEncKey;
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
	}
}


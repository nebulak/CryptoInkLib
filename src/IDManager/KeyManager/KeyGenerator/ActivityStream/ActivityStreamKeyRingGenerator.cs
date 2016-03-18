using System;
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

namespace CryptoInkLib
{
	public class ActivityStreamKeyRingGenerator
	{
		public ActivityStreamKeyRingGenerator ()
		{
		}

		//TODO: generate ActivityStreamKeyring
		public AsymmetricCipherKeyPair generateKeyring(int iKeySizeInBits)
		{
			RsaKeyPairGenerator _RSAKeyPairGenerator = new RsaKeyPairGenerator();
			//TODO: use secure PRNG
			_RSAKeyPairGenerator.Init(new KeyGenerationParameters(new SecureRandom(), iKeySizeInBits));
			AsymmetricCipherKeyPair userKey = _RSAKeyPairGenerator.GenerateKeyPair();

			PrivateKeyInfo privateKeyInfo = PrivateKeyInfoFactory.CreatePrivateKeyInfo(userKey.Private);
			byte[] serializedPrivateBytes = privateKeyInfo.ToAsn1Object().GetDerEncoded();
			return userKey;
		}

	}
}


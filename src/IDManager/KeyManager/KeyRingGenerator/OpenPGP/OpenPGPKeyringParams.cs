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
	//src: http://stackoverflow.com/questions/17953852/generating-pgp-key-ring-using-bouncy-castle-c-results-in-key-id-ffffffff
	public class KeyRingParams {

		public SymmetricKeyAlgorithmTag? PrivateKeyEncryptionAlgorithm{ get; set; }
		public SymmetricKeyAlgorithmTag[] SymmetricAlgorithms{ get; set; }
		public HashAlgorithmTag[] HashAlgorithms{ get; set; }
		public RsaKeyGenerationParameters RsaParams{ get; set; }
		public string Identity{ get; set; }
		public string Password{ get; set; }
		//= EncryptionAlgorithm.NULL;

		public char[] GetPassword() {
			return Password.ToCharArray();
		}

		public KeyRingParams() {
			//Org.BouncyCastle.Crypto.Tls.EncryptionAlgorithm
			RsaParams = new RsaKeyGenerationParameters(BigInteger.ValueOf(0x10001), new SecureRandom(), 2048, 12);
		}

	}
}


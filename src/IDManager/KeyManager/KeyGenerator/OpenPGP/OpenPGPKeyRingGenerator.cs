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
	public class OpenPGPKeyRingGenerator
	{
		public OpenPGPKeyRingGenerator ()
		{
		}

		public static OpenPGPRing generateKeyRing(String sIdentity, String sPassword)
		{
			PgpKeyRingGenerator keyRingGen = generateKeyRingGenerator (sIdentity, sPassword);
			PgpPublicKeyRing publicKeyRing = keyRingGen.GeneratePublicKeyRing ();
			PgpSecretKeyRing privateKeyRing = keyRingGen.GenerateSecretKeyRing ();

			OpenPGPRing openPgpRing = new OpenPGPRing (publicKeyRing, privateKeyRing);
			return openPgpRing;
		}

		//src: http://stackoverflow.com/questions/17953852/generating-pgp-key-ring-using-bouncy-castle-c-results-in-key-id-ffffffff

		public static PgpKeyRingGenerator generateKeyRingGenerator(String identity, String password) {

			KeyRingParams keyRingParams = new KeyRingParams();
			keyRingParams.Password = password;
			keyRingParams.Identity = identity;
			keyRingParams.PrivateKeyEncryptionAlgorithm = SymmetricKeyAlgorithmTag.Aes128;
			keyRingParams.SymmetricAlgorithms = new SymmetricKeyAlgorithmTag[] {
				SymmetricKeyAlgorithmTag.Aes256/*,
				SymmetricKeyAlgorithmTag.Aes192,
				SymmetricKeyAlgorithmTag.Aes128*/
				//TODO: delete ?
			};

			keyRingParams.HashAlgorithms = new HashAlgorithmTag[] {
				HashAlgorithmTag.Sha256,
				//TODO: delete ? + check Parameters
				//HashAlgorithmTag.Sha1,
				//HashAlgorithmTag.Sha384,
				HashAlgorithmTag.Sha512,
				//HashAlgorithmTag.Sha224,
			};

			IAsymmetricCipherKeyPairGenerator generator = GeneratorUtilities.GetKeyPairGenerator("RSA");
			generator.Init(keyRingParams.RsaParams);

			/* Create the master (signing-only) key. */
			PgpKeyPair masterKeyPair = new PgpKeyPair(
														PublicKeyAlgorithmTag.RsaSign,
														generator.GenerateKeyPair(),
														DateTime.UtcNow
													  );


			PgpSignatureSubpacketGenerator masterSubpckGen
			= new PgpSignatureSubpacketGenerator();
			masterSubpckGen.SetKeyFlags(false, 
										PgpKeyFlags.CanSign
										| PgpKeyFlags.CanCertify);

			//create var for preferred symmetric algorithms
			int [] iPreferedSymmetricAlgorithms = new int[keyRingParams.SymmetricAlgorithms.Length];
			for (int i = 0; i < keyRingParams.SymmetricAlgorithms.Length; i++) {
				iPreferedSymmetricAlgorithms[i] = (int) keyRingParams.SymmetricAlgorithms[i];
			}

			masterSubpckGen.SetPreferredSymmetricAlgorithms (false, iPreferedSymmetricAlgorithms);


			//create var for preferred hash algorithms
			int [] iPreferedHashAlgorithms = new int[keyRingParams.HashAlgorithms.Length];
			for (int i = 0; i < keyRingParams.HashAlgorithms.Length; i++) {
				iPreferedHashAlgorithms[i] = (int) keyRingParams.HashAlgorithms[i];
			}

			masterSubpckGen.SetPreferredHashAlgorithms (false, iPreferedHashAlgorithms);

			/* Create a signing and encryption key for daily use. */
			PgpKeyPair encKeyPair = new PgpKeyPair(
				PublicKeyAlgorithmTag.RsaGeneral,
				generator.GenerateKeyPair(),
				DateTime.UtcNow);


			PgpSignatureSubpacketGenerator encSubpckGen = new PgpSignatureSubpacketGenerator();
			encSubpckGen.SetKeyFlags(false, PgpKeyFlags.CanEncryptCommunications | PgpKeyFlags.CanEncryptStorage);

			masterSubpckGen.SetPreferredSymmetricAlgorithms(false, iPreferedSymmetricAlgorithms);
			masterSubpckGen.SetPreferredHashAlgorithms (false, iPreferedHashAlgorithms);

			/* Create the key ring. */
			PgpKeyRingGenerator keyRingGen = new PgpKeyRingGenerator(
				PgpSignature.DefaultCertification,
				masterKeyPair,
				keyRingParams.Identity,
				keyRingParams.PrivateKeyEncryptionAlgorithm.Value,
				keyRingParams.GetPassword(),
				true,
				masterSubpckGen.Generate(),
				null,
				new SecureRandom());

			/* Add encryption subkey. */
			keyRingGen.AddSubKey(encKeyPair, encSubpckGen.Generate(), null);

			return keyRingGen;
		}
	}
}


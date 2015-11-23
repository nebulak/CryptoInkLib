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
	public class Helpers
	{
		public Helpers ()
		{
		}

		public static string byteArrayToHexString(byte [] c_baInput)
		{
			string hex = BitConverter.ToString(c_baInput);

			return hex.Replace("-","");
		}
			

		public static byte[] hexStringToByteArray(String c_sInput)
		{
			int NumberChars = c_sInput.Length;
			byte[] bytes = new byte[NumberChars / 2];
			for (int i = 0; i < NumberChars; i += 2)
				bytes[i / 2] = Convert.ToByte(c_sInput.Substring(i, 2), 16);
			return bytes;
		}

		public static string createRandomString()
		{
			//create Random Number Generator
			var rng = new System.Security.Cryptography.RNGCryptoServiceProvider();

			//create random string
			byte[] randomBytes = new byte[32];
			rng.GetBytes (randomBytes);

			return byteArrayToHexString (randomBytes);
		}


	}
}


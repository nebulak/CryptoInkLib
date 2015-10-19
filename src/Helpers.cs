using System;
using Org.BouncyCastle;

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


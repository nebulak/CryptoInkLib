using System;
using System.IO;

namespace CryptoInkLib
{
	//Description: Encrypts and decrypts file contents with AES XTS 256 Bit
	public class FileContentCrypter
	{
		public FileContentCrypter ()
		{
		}
			
		public static Stream encryptStream(byte[] baKey1, byte[] baKey2, Stream plainTextStream)
		{
			var xts = XtsAes256.Create (baKey1, baKey2);

			var buffer = new byte[2048];
			//r.NextBytes(buffer);

			const int sectorSize = 512;

			using (plainTextStream) {
				using (var stream = new XtsSectorStream (plainTextStream, xts, sectorSize)) {
					int current = 0;
					while (current < buffer.Length) {
						var remaining = (buffer.Length - current);
						if (remaining > sectorSize)
							remaining = sectorSize;

						stream.Write (buffer, current, remaining);

						current += remaining;
					}

					return stream;
				}
			}
		}

		//TODO: implement function
		/*public byte[] decryptStreamPart(byte[] baKey1, byte[] baKey2, Stream cipherTextStream, int offset, int lengthInBytes)
		{

		}*/



		public static Stream decryptStream(byte[] baKey1, byte[] baKey2, Stream cipherTextStream)
		{
			var xts = XtsAes256.Create (baKey1, baKey2);

			//buffer for plaintext-bytes
			var buffer = new byte[cipherTextStream.Length];
			//r.NextBytes(buffer);

			const int sectorSize = 512;

			using (cipherTextStream) {
				using (var stream = new XtsSectorStream (cipherTextStream, xts, sectorSize)) {
					int current = 0;
					//TODO: ....
					/*
					while (current < buffer.Length) {
						var remaining = (buffer.Length - current);
						if (remaining > sectorSize)
							remaining = sectorSize;

						stream.Read (buffer, current, remaining);

						current += remaining;
					}

					return buffer;*/
					return stream;
				}
			}
		}


	}
}


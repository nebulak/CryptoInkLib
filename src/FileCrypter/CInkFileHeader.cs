using System;

namespace CryptoInkLib
{
	public class CInkFileHeader
	{
		public CInkFileHeader ()
		{
		}

		public CInkFileHeader (byte[] content_iv, EncryptedSessionKey[] content_keys, byte[] filename_iv, EncryptedSessionKey[] filename_keys)
		{
			this.content_iv = content_iv;
			this.content_keys = content_keys;
			this.filename_iv = filename_iv;
			this.filename_keys = filename_keys;
		}


		public byte[] 					content_iv;
		public EncryptedSessionKey[] 	content_keys;

		public byte[] 					filename_iv;
		public EncryptedSessionKey[] 	filename_keys;

	}
}


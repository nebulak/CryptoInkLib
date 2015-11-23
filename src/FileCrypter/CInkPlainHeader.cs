using System;

namespace CryptoInkLib
{
	public class CInkPlainHeader
	{
		public CInkPlainHeader ()
		{
		}

		public CInkPlainHeader (byte[] content_iv, SessionKey[] content_keys, byte[] filename_iv, SessionKey[] filename_keys)
		{
			this.content_iv = content_iv;
			this.content_keys = content_keys;
			this.filename_iv = filename_iv;
			this.filename_keys = filename_keys;
		}

		public byte[] 					content_iv;
		public SessionKey[] 			content_keys;

		public byte[] 					filename_iv;
		public SessionKey[] 			filename_keys;
	}
}


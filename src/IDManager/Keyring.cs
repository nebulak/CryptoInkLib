using System;

namespace CryptoInkLib
{
	public class Keyring
	{
		public Keyring ()
		{
		}

		public EKeyringType keyringType { get; set; }
		public object keyringContent { get; set; }
	}
}

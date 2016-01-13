using System;

namespace CryptoInkLib
{
	public class KeyStoreStorage
	{
		public KeyStoreStorage ()
		{

		}

		public Key[] 	friendUserKeys { get; set; }
		public Key[] 	userKeys { get; set; }
	}
}


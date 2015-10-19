using System;

namespace CryptoInkLib
{
	public class KeyStoreStorage
	{
		public KeyStoreStorage ()
		{

		}

		public FriendUserKey[] 	friendUserKeys { get; set; }
		public UserKey[] 	userKeys { get; set; }
	}
}


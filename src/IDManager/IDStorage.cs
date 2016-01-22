using System;

namespace CryptoInkLib
{
	public class IDStorage
	{
		public IDStorage ()
		{

		}

		public KeyBox 		 keyBox { get; set; }
		//TODO: implement Contact, Settings and readd following lines
		//public List<Contact> contacts { get; set; }
		//public Settings settings { get; set; }
		//public byte[] conversationDBKey { get; set; } //for CryptoSQLite

		//TODO: delete the following 2 key arrays...
		public Key[] 	publicKeys { get; set; }
		public Key[] 	privateKeys { get; set; }
	}
}


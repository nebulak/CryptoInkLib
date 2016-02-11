using System;
using System.Collections.Generic;

namespace CryptoInkLib
{
	public class IDStorage
	{
		public IDStorage ()
		{

		}
			
		public List<Keyring> keyBox { get; set; }
		//TODO: implement Contact, Settings and readd following lines
		//public List<Contact> contacts { get; set; }
		//public Settings settings { get; set; }
		//public byte[] conversationDBKey { get; set; } //for CryptoSQLite

	}
}


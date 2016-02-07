using System;
using System.Collections.Generic;

namespace CryptoInkLib
{
	public  struct Contact
	{
		public string sNickname;
		public string sId; //Email-address/JID

		public List<string> supportedProtocols;
	}
}
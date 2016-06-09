using System;
using System.Collections.Generic;

namespace CryptoInkLib
{
	public  struct Contact
	{
		public string nickname;
		public string id;
		public string presence_type;
		public string presence_text;
		public DateTime presence_date;
		public bool? bIsEmailSupported;
		public bool? bIsXmppSupported;
	}
}
using System;
using System.Collections.Generic;

namespace CryptoInkLib
{
	public class WebsocketRequest
	{
		public WebsocketRequest ()
		{
		}

		public string command;
		public Dictionary<string, string> parameters;
	}
}


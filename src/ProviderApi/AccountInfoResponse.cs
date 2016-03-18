using System;
using System.Collections.Generic;

namespace CryptoInkLib
{
	public class AccountInfoResponse
	{
		public AccountInfoResponse ()
		{
		}

		public string username;
		public ServiceLevel level;
		public List<string> payment_options;
		public DateTime is_valid_until;
	}
}


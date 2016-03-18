using System;
using System.Collections.Generic;

namespace CryptoInkLib
{
	public class ServiceLevel
	{
		public ServiceLevel ()
		{
			
		}

		public string name;
		public Dictionary<string, string> description;
		public double price_per_month;
		public string currency;
		public int free_months;
	}
}


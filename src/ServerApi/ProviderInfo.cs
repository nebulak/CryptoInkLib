using System;
using System.Collections.Generic;

namespace CryptoInkLib
{
	public class ProviderInfo
	{
		
		public string api_uri;
		public string api_version;
		public string default_language;
		public Dictionary<string, string> description;
		public string domain;
		public string enrollment_policy;
		public List<string> languages;
		public string name { get; set; }
		public List<string> payment_options;
		public List<ServiceLevel> levels { get; set; }
		public List<string> services { get; set; }
	}
}


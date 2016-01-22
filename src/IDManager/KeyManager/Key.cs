using System;

namespace CryptoInkLib
{
	public class Key
	{
		public Key ()
		{
		}

		//Key
		public string 	keyID 			{ get; set; }
		public string	type			{ get; set; }
		public int 		status 			{ get; set; } // 0 - Revoked, 1 - Active
		public object 	keyContent		{ get; set; } // stores the key Object e.g. PGPKey ...
		public SService service			{ get; set; } //Info about the service, the key is used for
	}
}


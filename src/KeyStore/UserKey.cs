using System;

namespace CryptoInkLib
{
	public class UserKey
	{
		public UserKey ()
		{
		}

		//Keys are stored in BASE64PEM-Format
		public string 	keyID 			{ get; set; }
		public string 	publicKey 		{ get; set; }
		public string 	privateKey 		{ get; set; }
		public int 		status 			{ get; set; } // 0 - Revoked, 1 - Active
		public string 	creationDate 	{ get; set; } // CreationDate
		public string 	revokationDate	{ get; set; } //TODO: add to keycreation and other classes
		public SService service			{ get; set; } //Info about the service, the key is used for
	}
}


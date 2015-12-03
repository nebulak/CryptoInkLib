using System;

namespace CryptoInkLib
{
	public class FriendUserKey
	{
		public FriendUserKey ()
		{
		}

		//Keys are stored in BASE64PEM-Format
		public string 	keyID 			{ get; set; }
		public string 	publicKey 		{ get; set; }
		public int 		status 			{ get; set; } // 0 - Revoked, 1 - Active
		public string 	creationDate 	{ get; set; } // CreationDate
		public string 	revokationDate	{ get; set; }

	}
}


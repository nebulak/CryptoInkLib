using System;

namespace CryptoInkLib
{
	public struct SRemoteService
	{
		public string 	url				{ get; set; } //The URL of the service
		public string 	port			{ get; set; } //The Port which is used by the service
		public string	authUser		{ get; set; } //The name used for authentication against the service
		public string	authPassword	{ get; set; } //The password used for authentication against the service
		public string 	authType		{ get; set; } //The authentication type used for the service
		//TODO: new struct for handling different auth types
	}
}


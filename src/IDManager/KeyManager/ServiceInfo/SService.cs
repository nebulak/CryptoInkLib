using System;

namespace CryptoInkLib
{
	public struct SService
	{
		public EServiceType 	type				{ get; set; } //The service as string e.g. file, xmpp, jabber
		public bool 			usesRemoteService 	{ get; set; } //Indicates if a remote service is used
		public SRemoteService 	remoteService { get; set; } //Info about the remote service
	}
		
}


using System;
using WebSocketSharp;
using WebSocketSharp.Net;
using WebSocketSharp.Server;


namespace CryptoInkLib
{
	public class WebsocketService
	{
		public WebsocketService (CommandParser commandParser)
		{
			m_WebSocketServer = new WebSocketServer (System.Net.IPAddress.Parse ("127.0.0.1"), 5963, null, AuthenticationSchemes.None);
			m_WebSocketServer.AddWebSocketService<WsServiceBehaviour> ("/service", () => new WsServiceBehaviour (commandParser));
			m_WebSocketServer.Start ();
		}

		private WebSocketServer m_WebSocketServer;

		public void stop()
		{
			m_WebSocketServer.Stop ();
		}
			
	}
}


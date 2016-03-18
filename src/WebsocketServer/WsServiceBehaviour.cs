using System;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace CryptoInkLib
{
	public class WsServiceBehaviour : WebSocketBehavior
	{
		public WsServiceBehaviour () : this (null)
		{

		}

		public WsServiceBehaviour (CommandParser commandParser)
		{
			m_CommandParser = commandParser;
		}

		public CommandParser m_CommandParser;



		protected override System.Threading.Tasks.Task OnMessage (MessageEventArgs e)
		{
			
			string sResponse = m_CommandParser.parseRequest (e.Text.ToString());

			return Send (sResponse);
		}
	}
}


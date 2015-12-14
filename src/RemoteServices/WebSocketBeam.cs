using System;
using WebSocketSharp;
using WebSocketSharp.Server;
using Newtonsoft.Json;

namespace CryptoInkLib
{
	public class WebSocketBeam : WebSocketBehavior
	{
		public WebSocketBeam ()
		{
			
		}

		protected override void OnMessage (MessageEventArgs e)
		{
			var msg = e.Data == "BALUS"
				? "I've been balused already..."
				: "I'm not available now.";

			Send (msg);
		}

		public void beam(BeamMessage beamMessage)
		{
			string sMessage = JsonConvert.SerializeObject(beamMessage);
			Send (sMessage);
		}


	}
}


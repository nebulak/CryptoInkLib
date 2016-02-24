using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace CryptoInkLib
{
	public class CommandParser
	{
		public CommandParser ()
		{
			
		}


		public string parseRequest(string sRequest)
		{
			WebsocketRequest request = JsonConvert.DeserializeObject<WebsocketRequest>(sRequest);

			switch (request.command) 
			{
			case "getContacts":
				this.getContacts ();
			case "getConversations":
				this.getConversations ();									
			case "getNotifications":
				this.getNotifications ();
			case "getMessengerStatus":
				this.getMessengerStatus ();
			case "sendMessage":
				this.sendMessage (request.parameters);
			}

				
		}


		public string getContacts()
		{

		}

		public string getConversations()
		{

		}

		public string getNotifications()
		{

		}

		public string getMessengerStatus()
		{

		}

		public string sendMessage(Dictionary<string, string> dParameters)
		{
			if (dParameters.ContainsKey ("receiver") && dParameters.ContainsKey ("message")) {
				//TODO: implement send message
			}
		}
	}
}


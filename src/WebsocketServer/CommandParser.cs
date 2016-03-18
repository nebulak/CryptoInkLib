using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace CryptoInkLib
{
	public class CommandParser
	{
		public CommandParser (MessagingManager messagingManager)
		{
			
		}

		public MessagingManager m_MessagingManager;


		public string parseRequest(string sRequest)
		{
			WebsocketRequest request = JsonConvert.DeserializeObject<WebsocketRequest>(sRequest);

			switch (request.command) 
			{
			case "getContacts":
				return this.getContacts ();
			case "getConversations":
				return this.getConversations ();									
			case "getNotifications":
				return this.getNotifications ();
			case "getMessengerStatus":
				return this.getMessengerStatus ();
			case "sendMessage":
				return this.sendMessage (request.parameters);
			}
			//TODO: implement all fnctions
			return "";
		}

		//TODO: implement function
		public string getContacts()
		{
			return "";
		}

		public string getConversations()
		{
			return JsonConvert.SerializeObject (m_MessagingManager.m_ConversationManager.m_Conversations);
		}

		//TODO: implement function
		public string getNotifications()
		{
			return "";
		}

		//TODO: implement function
		public string getMessengerStatus()
		{
			return "";
		}


		//
		// actions
		//

		//TODO: implement function
		public string addContact(Dictionary<string, string> dParameters)
		{
			return "";
		}

		//TODO: add Post to ActivityStream
		public string addPost(Dictionary<string, string> dParameters)
		{
			return "";
		}

		public string sendMessage(Dictionary<string, string> dParameters)
		{
			if (dParameters.ContainsKey ("receiver") && dParameters.ContainsKey ("message")) {
				//TODO: implement send message
			}
			return "";
		}
	}
}


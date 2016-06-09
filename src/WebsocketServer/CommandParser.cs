using System;
using System.Collections.Generic;
using agsXMPP;
using agsXMPP.protocol.client;
using agsXMPP.Collections;
using agsXMPP.protocol.iq.roster;
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

		//TODO: test if it works
		public string getContacts()
		{
			Dictionary<string, string> dContacts = m_MessagingManager.m_XmppManager.getContacts();
			Dictionary<string, Presence> dPresence = m_MessagingManager.m_XmppManager.getContactPresences ();
			List<ContactModel> contactList = new List<ContactModel> ();

			foreach (var item in dContacts) {
				ContactModel currentContact = new ContactModel ();
				currentContact.id = item.Value;
				currentContact.nickname = item.Key;
				currentContact.presence_type = dPresence [item.Key].Type.ToString ();
				currentContact.presence_text = dPresence [item.Key].Status;

				contactList.Add (currentContact);
			}

			return JsonConvert.SerializeObject(contactList);
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


		public string sendMessage(Dictionary<string, string> dParameters)
		{
			if (dParameters.ContainsKey ("receiver") && dParameters.ContainsKey ("message")) {
				//TODO: implement send message
			}
			return "";
		}
	}
}


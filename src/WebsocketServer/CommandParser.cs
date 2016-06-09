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
				return JsonConvert.SerializeObject(this.getContacts ());
			case "getConversations":
				return JsonConvert.SerializeObject(this.getConversations ());									
			case "getNotifications":
				return JsonConvert.SerializeObject(this.getNotifications ());
			case "getMessengerStatus":
				return JsonConvert.SerializeObject(this.getMessengerStatus ());
			case "sendMessage":
				return JsonConvert.SerializeObject(this.sendMessage (request.parameters));
			}
			//TODO: implement all fnctions
			return "";
		}

		//TODO: test if it works
		public List<Contact> getContacts()
		{
			Dictionary<string, string> dContacts = m_MessagingManager.m_XmppManager.getContacts();
			Dictionary<string, Presence> dPresence = m_MessagingManager.m_XmppManager.getContactPresences ();
			List<Contact> contactList = new List<Contact> ();

			foreach (var item in dContacts) {
				Contact currentContact = new Contact ();
				currentContact.id = item.Value;
				currentContact.nickname = item.Key;
				currentContact.presence_type = dPresence [item.Key].Type.ToString ();
				currentContact.presence_text = dPresence [item.Key].Status;

				contactList.Add (currentContact);
			}

			return contactList;
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
			//TODO: move complex part to MessagingManager
			if (dParameters.ContainsKey ("receiver") && dParameters.ContainsKey ("message")) {
				//TODO: implement send message
				List<Contact> contacts = getContacts();

				//check if we have the recipient in the addressbook
				foreach (var item in contacts) {
					if (item.id == dParameters ["receiver"]) {
						//TODO: use try-catch-block
						//check if we can send xmpp 
						if (item.bIsXmppSupported) {
							//TODO: check if XmppManager can send OTR and PGP encrypted
							m_MessagingManager.m_XmppManager.sendMessage(dParameters ["message"], dParameters["receiver"]);
						} else {
							m_MessagingManager.m_EmailManager.sendMessage (dParameters ["message"], dParameters ["receiver"]);
						}
					}
				}
			}
			//TODO: add return message
			return "";
		}
	}
}


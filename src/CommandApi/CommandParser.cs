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
			case "getAddressBook":
				return JsonConvert.SerializeObject(this.getAddressBook ());
			case "addContact":
				return JsonConvert.SerializeObject(this.addContact(request.parameters));
			case "updateContact":
				return JsonConvert.SerializeObject(this.updateContact(request.parameters));
			case "deleteContact":
				return JsonConvert.SerializeObject(this.updateContact(request.parameters));
			case "getAccount":
				return "";
			case "updateAccount":
				return "";
			case "deleteAccount":
				return "";
			case "addAccount":
				return "";
			case "getConversations":
				return JsonConvert.SerializeObject(this.getConversations ());
			case "getConversationWith":
				return "";
			case "getNotifications":
				return JsonConvert.SerializeObject(this.getNotifications ());
			case "getMessengerStatus":
				return JsonConvert.SerializeObject(this.getMessengerStatus ());
			case "sendIM":
				return JsonConvert.SerializeObject(this.sendIM (request.parameters));
			case "sendMail":
				return JsonConvert.SerializeObject(this.sendMail (request.parameters));
			}
			//TODO: implement all functions
			return "";
		}


		public List<Contact> getAddressBook()
		{
			return m_MessagingManager.m_AddressBook.m_Contacts;
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
			if (!(
				dParameters.ContainsKey ("sNickname") &&
				dParameters.ContainsKey("sEmail") &&
				dParameters.ContainsKey("sNote") 
			)) {
				//TODO: throw exception
				return "";
			}

			//TODO: try to subscribe using xmpp

			//TODO: try to discover openpgp key (and otr-key)

			Contact newContact = new Contact ();
			newContact.sNickname = dParameters ["sNickname"];
			newContact.sId = dParameters ["sEmail"];
			newContact.communicationProtocols.Add (ECommunicationProtocol.EMAIL);
			newContact.sNote = dParameters ["sNote"];


			this.m_MessagingManager.m_AddressBook.addContact (newContact);
			return "";
		}


		public string updateContact(Dictionary<string, string> dParameters)
		{
			if ( 
				! (dParameters.ContainsKey ("sOldNickname") && 
				dParameters.ContainsKey ("sNewNickname") &&
				dParameters.ContainsKey("sNewId") &&
				dParameters.ContainsKey("sNewNote")
			)) {
				//TODO: throw exception
				return "";
			}
			Contact oldContact = this.m_MessagingManager.m_AddressBook.getContactByNickname (dParameters["sOldNickname"]);
			oldContact.sId = dParameters["sNewId"];
			oldContact.sNickname = dParameters["sNewNickname"];
			oldContact.sNote = dParameters["sNewNote"];

			this.m_MessagingManager.m_AddressBook.save();
			//TODO:
			return "";
		}


		public string sendIM(Dictionary<string, string> dParameters)
		{
			string sReceiverNickname = "";
			string sMessage = "";

			if ( !(dParameters.ContainsKey ("receiver") && dParameters.ContainsKey ("message")) ) {
				//TODO: throw Exception
				return "";
			}
			sReceiverNickname = dParameters ["receiver"];
			sMessage = dParameters ["message"];

			return this.m_MessagingManager.sendIM (sReceiverNickname, sMessage);
		}



		public string sendMail(Dictionary<string, string> dParameters)
		{
			//TODO: implement
			return "";
		}
	}
}


using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace CryptoInkLib
{
	public class ConversationManager
	{
		public ConversationManager ()
		{
			this.m_Conversations = new List<Conversation>();
		}

		public string m_OwnJid;
		public List<Conversation> m_Conversations;


		public Conversation getConversationWith(string sPartner)
		{
			for (int i = 0; i < m_Conversations.Count; i++) {
				if (m_Conversations [i].m_sPartner == sPartner) {
					return m_Conversations [i];
				}
			}
			return null;
		}


		public void addMessage(string sProtocol, string sMessage, string sFrom, string sTo)
		{
			Message message = new Message ();
			message.m_SenderId = sFrom;
			message.m_ReceiverId = sTo;
			message.m_Message = sMessage;
			message.m_Date = DateTime.Now;

			string sPartner = "";

			if (sFrom == m_OwnJid) {
				sPartner = sTo;
			} else {
				sPartner = sFrom;
			}

			Conversation conversation =  this.getConversationWith (sPartner);
			if (conversation == null) {
				conversation = new Conversation ();
				conversation.m_sPartner = sPartner;
				if (conversation.m_Conversation == null) {
					conversation.m_Conversation = new List<Message> ();
				}
				conversation.m_Conversation.Add (message);
				m_Conversations.Add (conversation);
			} else {
				conversation.m_sPartner = sPartner;
				if (conversation.m_Conversation == null) {
					conversation.m_Conversation = new List<Message> ();
				}
				conversation.m_Conversation.Add (message);
			}
		}

	}
}


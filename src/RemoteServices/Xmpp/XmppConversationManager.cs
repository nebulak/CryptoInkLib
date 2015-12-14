using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace CryptoInkLib
{
	public class XmppConversationManager
	{
		public XmppConversationManager (WebSocketBeam _websocketBeam)
		{
			this.m_Conversations = new List<XmppConversation>();
			this.m_Beam = _websocketBeam;
		}

		public string m_OwnJid;
		public List<XmppConversation> m_Conversations;
		private WebSocketBeam m_Beam;

		public XmppConversation getConversationWith(string sPartner)
		{
			for (int i = 0; i < m_Conversations.Count; i++) {
				if (m_Conversations [i].m_sPartner == sPartner) {
					return m_Conversations [i];
				}
			}
			return null;
		}

		public void addMessage(string sMessage, string sFrom, string sTo)
		{
			XmppMessage xmppMessage = new XmppMessage ();
			xmppMessage.m_SenderJid = sFrom;
			xmppMessage.m_ReceiverJid = sTo;
			xmppMessage.m_Message = sMessage;
			xmppMessage.m_Date = new DateTime ();

			string sPartner = "";

			if (sFrom == m_OwnJid) {
				sPartner = sTo;
			} else {
				sPartner = sFrom;
			}

			XmppConversation xmppConversation =  this.getConversationWith (sPartner);
			if (xmppConversation == null) {
				xmppConversation = new XmppConversation ();
				xmppConversation.m_sPartner = sPartner;
			} else {
				this.m_Conversations.Remove (xmppConversation);
			}
			xmppConversation.m_Conversation.Add (xmppMessage);
			BeamMessage beamMessage = new BeamMessage ();
			beamMessage.sFromService = "xmpp";
			beamMessage.sAction = "addMessage";
			//TODO: implement correctly
			beamMessage.sMessage = JsonConvert.SerializeObject(xmppMessage);
		}

	}
}


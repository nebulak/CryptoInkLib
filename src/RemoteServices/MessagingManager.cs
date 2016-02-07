using System;
using System.Collections.Generic;

namespace CryptoInkLib
{
	public class MessagingManager
	{
		public MessagingManager (	Dictionary<string, string> mailConfigDictionary,
									Dictionary<string, string> xmppConfigDictionary,
									int inboxCheckIntervall,
									bool isInMinutes)
		{
			m_InboxCheckIntervall = inboxCheckIntervall;
			m_bIsIntervallInMinutes = isInMinutes;
			m_ConversationManager = new ConversationManager ();

			m_EmailManager = emailManager;
			m_XmppManager = xmppManager;
		}


		public EmailManager m_EmailManager;
		public XmppManager m_XmppManager;
		public ConversationManager m_ConversationManager;
		public Logger m_Logger;
		public int m_InboxCheckIntervall;
		public bool m_bIsIntervallInMinutes;

		//TODO: return type should be Error code
		private int initXmppManager(Dictionary<string, string> dConfig)
		{
			if ((dConfig ["sJid"] == null) || (dConfig ["sPassword"] == null)) {
				//TODO: return Error code
				return 1;
			}
			m_XmppManager = new XmppManager (dConfig ["sJid"], dConfig ["sPassword"], m_ConversationManager, m_Logger);
			//TODO: return type should be error code
			return 0;
		}


	}
}


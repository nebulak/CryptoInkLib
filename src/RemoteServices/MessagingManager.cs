using System;
using System.Collections.Generic;

namespace CryptoInkLib
{
	public class MessagingManager
	{
		public MessagingManager (	AuthInfo authInfo,
									EmailServiceDescription emailServiceDescription,
									OpenPGPRing openPgpRing,
									XmppServiceDescription xmppServiceDescription,
									OTRKeyRing otrKeyring,
									ContactManager contactManager,
									Logger logger,
									int inboxCheckIntervall,
									bool isInMinutes)
		{
			m_AuthInfo = authInfo;
			m_EmailServiceDescription = emailServiceDescription;
			m_OpenPgpRing = openPgpRing;
			m_XmppServiceDescription = xmppServiceDescription;
			m_OtrKeyRing = otrKeyring;
			m_Logger = logger;
			m_InboxCheckIntervall = inboxCheckIntervall;
			m_bIsIntervallInMinutes = isInMinutes;
			m_ConversationManager = new ConversationManager ();
			m_ContactManager = contactManager;

			RC rcMail = initEmailManager();
			RC rcXmpp = initXmppManager();
			m_Status = RC.RC_OK;
			if (rcMail != RC.RC_OK) {
				m_Status = RC.RC_COULD_NOT_INIT_EMAIL;
			}

			if (rcXmpp != RC.RC_OK) {
				m_Status = RC.RC_COULD_NOT_INIT_XMPP;
			}
		}

		private AuthInfo m_AuthInfo;
		public EmailServiceDescription m_EmailServiceDescription;
		private OpenPGPRing m_OpenPgpRing;
		private OTRKeyRing m_OtrKeyRing;
		public XmppServiceDescription m_XmppServiceDescription;
		public EmailManager m_EmailManager;
		public XmppManager m_XmppManager;
		public ConversationManager m_ConversationManager;
		public ContactManager m_ContactManager;
		public Logger m_Logger;
		public int m_InboxCheckIntervall;
		public bool m_bIsIntervallInMinutes;
		public RC m_Status;


		private RC initXmppManager()
		{
			if (m_XmppServiceDescription == null) {
				return RC.RC_COULD_NOT_INIT_XMPP;
			}
			m_XmppManager = new XmppManager (m_AuthInfo, m_XmppServiceDescription, m_OtrKeyRing, m_OpenPgpRing, m_ConversationManager, m_ContactManager, m_Logger);
			return RC.RC_OK;
		}
			

		private RC initEmailManager()
		{
			if (m_EmailServiceDescription == null) {
				return RC.RC_COULD_NOT_INIT_EMAIL;
			}
			m_EmailManager = new EmailManager (m_AuthInfo, m_EmailServiceDescription, m_OpenPgpRing, m_ConversationManager, m_Logger);
			return RC.RC_OK;
		}


		//TODO: implement
//		public RC sendMessage(string sReceiverId, string sMessage)
//		{
//			if(m_XmppManager.
//		}

	}
}


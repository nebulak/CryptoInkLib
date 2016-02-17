using System;
using System.Collections.Generic;

namespace CryptoInkLib
{
	public class MessagingManager
	{
		//TODO: use PGPRing and add OTR-Key
		public MessagingManager (	AuthInfo authInfo,
									EmailServiceDescription emailServiceDescription,
									OpenPGPRing openPgpRing,
									XmppServiceDescription xmppServiceDescription,
									Logger logger,
									int inboxCheckIntervall,
									bool isInMinutes)
		{
			m_AuthInfo = authInfo;
			m_EmailServiceDescription = emailServiceDescription;
			m_OpenPgpRing = openPgpRing;
			m_XmppServiceDescription = xmppServiceDescription;
			m_Logger = logger;
			m_InboxCheckIntervall = inboxCheckIntervall;
			m_bIsIntervallInMinutes = isInMinutes;
			m_ConversationManager = new ConversationManager ();

			RC rcMail = initEmailManager();
			RC rcXmpp = initXmppManager();
			if ((rcMail != RC.RC_OK) || (rcXmpp != RC.RC_OK)) {
				//TODO: throw Exception
			}
		}

		private AuthInfo m_AuthInfo;
		public EmailServiceDescription m_EmailServiceDescription;
		private OpenPGPRing m_OpenPgpRing;
		public XmppServiceDescription m_XmppServiceDescription;
		public EmailManager m_EmailManager;
		public XmppManager m_XmppManager;
		public ConversationManager m_ConversationManager;
		public Logger m_Logger;
		public int m_InboxCheckIntervall;
		public bool m_bIsIntervallInMinutes;


		private RC initXmppManager()
		{
			if (m_XmppServiceDescription == null) {
				return RC.RC_COULD_NOT_INIT_XMPP;
			}
			m_XmppManager = new XmppManager (m_AuthInfo, m_XmppServiceDescription, m_ConversationManager, m_Logger);
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

	}
}


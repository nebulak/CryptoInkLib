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
									AddressBook addressBook,
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
			m_AddressBook = addressBook;

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
		public AddressBook m_AddressBook;
		public Logger m_Logger;
		public int m_InboxCheckIntervall;
		public bool m_bIsIntervallInMinutes;
		public RC m_Status;


		private RC initXmppManager()
		{
			if (m_XmppServiceDescription == null) {
				return RC.RC_COULD_NOT_INIT_XMPP;
			}
			m_XmppManager = new XmppManager (m_AuthInfo, m_XmppServiceDescription, m_OtrKeyRing, m_OpenPgpRing, m_ConversationManager, m_AddressBook, m_Logger);
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


		public void getEmails()
		{
			//TODO: m_EmailManager.getMessages ();
		}



		public string sendIM(string sReceiverNickname, string sMessage)
		{
			//get account for recipient from AddressBook
			Contact receiverContact = this.m_AddressBook.getContactByNickname(sReceiverNickname);

			string sXmppId = "";
			ECryptoProtocol? xmppCryptoProtocol = null;


			//TODO: only start messaging after we checked all options for communication
			if (receiverContact.communicationProtocols.Contains(ECommunicationProtocol.XMPP)) {
				if (this.m_XmppManager.m_PresenceManager.isContactOnline (receiverContact.sId)) {
					sXmppId = receiverContact.sId;
					if (receiverContact.cryptoProtocols.Count != 0) {
						//contact is online and supports encryption!
						//so let's try if we can use OTRv3 or v2
						try {
							this.m_XmppManager.sendMessage (sXmppId, sMessage);
							//TODO: return value
							return "";
						} catch (Exception e) {
							//TODO:
							return "";
						}

					} else {
						//TODO: Test if OTR is possible, if yes also add it to encryption protocols
						try {
							this.m_XmppManager.sendMessage (sXmppId, sMessage);
							//TODO: return value
							return "";
						} 
						catch (Exception e) {
							//TODO: send message using openpgp
							return "";
						}
					}
				} else {
					//contact is not online, use pgp over xmpp
					try {
						//TODO: send openpgp encrypted
						//TODO: return value
						return "";
					} catch (Exception e) {
						return "";
					}
				}
			}

			return "";
			//TODO: return error
		}


		public RC subscribeXmpp(string sJid)
		{
			//TODO: implement
			return RC.RC_OK;
		}


		public RC answerSubscriptionRequest(bool bAcceptRequest, bool doSubscription)
		{
			//TODO: implement
			return RC.RC_OK;
		}

	}
}


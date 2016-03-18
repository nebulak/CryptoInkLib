using System;
using System.Collections.Generic;
using agsXMPP;
using agsXMPP.protocol.client;
using agsXMPP.Collections;
using agsXMPP.protocol.iq.roster;
using System.Threading;
using MonoOTRLib;
using Mono.OTR.Interface;


namespace CryptoInkLib
{
	/// <summary>
	/// The XmppManager class establishs and manages a connection to a Jabber-Server
	/// </summary>
	public class XmppManager
	{
		public const int OTR_STATE_CREATE_SESSION = 0;
		public const int OTR_STATE_REQUEST_SESSION = 1;
		public const int OTR_STATE_READY = 2;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sJid">S jid.</param>
		/// <param name="sPassword">S password.</param>
		public XmppManager (AuthInfo authInfo, XmppServiceDescription xmppServiceDescription, OTRKeyRing _OTRKeyRing, OpenPGPRing _OpenPGPRing, ConversationManager _conversationManager, ContactManager _contactMgr, Logger _logger)
		{
			try
			{
				m_Logger = _logger;
				m_OTRKeyRing = _OTRKeyRing;
				m_OpenPGPRing = _OpenPGPRing;
				m_AuthInfo = authInfo;
				m_XmppServiceDescription = xmppServiceDescription;
				m_JidSender = new Jid (m_AuthInfo.m_sId);
				m_ClientConnection = new XmppClientConnection(m_JidSender.Server);
				m_Contacts = new Dictionary<string, string> ();
				m_Logger.log(ELogLevel.LVL_INFO, "Trying to log in xmpp user", m_sModuleName);
				m_ClientConnection.Open(m_JidSender.User, m_AuthInfo.m_sPassword);
				m_ConversationManager = _conversationManager;
				m_ContactManager = _contactMgr;
				m_OtrConnections = new Dictionary<string, int>();

				//register EventHandlers
				m_ClientConnection.OnLogin += new ObjectHandler(onLogin);
				m_ClientConnection.OnPresence += new PresenceHandler(OnPresence);
			}
			catch(Exception e) {
				Console.Write (e.Message);
			}


			//info: message callback is registered in onRosterItem callback
		}

		private XmppServiceDescription m_XmppServiceDescription;
		private AuthInfo m_AuthInfo;
		private OTRKeyRing m_OTRKeyRing;
		private OpenPGPRing m_OpenPGPRing;
		private ContactManager m_ContactManager;
		private Logger m_Logger;
		
		public static string m_sProtocolName = "xmpp";
		
		public static string m_sModuleName = "XmppManager";

		public bool m_isInstantMessageProtocol {
			get { return true; }
		}

		/// <summary>
		/// The m jid sender stores the JID for the loggedIn user.
		/// </summary>
		private Jid m_JidSender;

		/// <summary>
		/// The otr session manager.
		/// </summary>
		private OTRSessionManager m_OtrSessionManager;


		private Dictionary<string, int> m_OtrConnections;


		/// <summary>
		/// Dictionary for storing Jid-Username.
		/// </summary>
		private Dictionary<string, string> m_Contacts;

		/// <summary>
		/// The conversation manager for storing a users conversations.
		/// </summary>
		private ConversationManager m_ConversationManager;

		/// <summary>
		/// The m client connection stores the Jabber-Connection of the user.
		/// </summary>
		private XmppClientConnection m_ClientConnection;

		/// <summary>
		/// The m is logged in stores a bool to indicate if the user is logged in.
		/// </summary>
		private bool m_IsLoggedIn;

		/// <summary>
		/// The m is authenticated stores a bool to indicate if the user is successfully authenticated.
		/// </summary>
		private bool m_IsAuthenticated;

		/// <summary>
		/// The state of the XmppConnection.
		/// </summary>
		private XmppConnectionState m_ConnectionState;

		/// <summary>
		/// The m presence stores the presence of the user.
		/// </summary>
		private Presence m_Presence;

		/// <summary>
		/// handles the OnLogin-event and updates the Presence of the user
		/// </summary>
		/// <param name="oSender">The object which sent the event.</param>
		private void onLogin(object oSender)
		{
			
			try 
			{
				m_IsAuthenticated = m_ClientConnection.Authenticated;
				m_ConnectionState = m_ClientConnection.XmppConnectionState;

				//check if user is authenticated
				if (m_IsAuthenticated && m_ConnectionState == XmppConnectionState.Authenticated)
				{
					m_IsLoggedIn = true;
					m_Logger.log(ELogLevel.LVL_INFO, "Xmpp user is logged in", m_sModuleName);
				}
				else
				{
					m_Logger.log(ELogLevel.LVL_INFO, "Xmpp user could not be authenticated or is not connected.", m_sModuleName);
					m_ClientConnection.Open(m_JidSender.User, m_AuthInfo.m_sPassword);
					return;
				}

				m_Presence = new Presence (ShowType.chat, "Online");
				m_Presence.Type = PresenceType.available;
				m_ConversationManager.m_OwnJid = this.m_JidSender;
				m_ClientConnection.Send(m_Presence);
				m_ClientConnection.OnRosterItem += new XmppClientConnection.RosterHandler(onRosterItem);
				m_ClientConnection.OnMessage += new MessageHandler(handleMessage);
				m_OtrSessionManager = new OTRSessionManager(m_JidSender);
				m_OtrSessionManager.OnOTREvent += new OTREventHandler(OnOTREvent);
			}
			catch(Exception e) {
				m_Logger.log(ELogLevel.LVL_WARNING, "onLogin: " + e.Message, m_sModuleName);
			}
		}

		private void onRosterItem(object sender, RosterItem item)
		{
			m_Logger.log(ELogLevel.LVL_TRACE, "Add new Roster item!", m_sModuleName);
			m_Contacts.Add(item.GetAttribute("jid").ToString(), item.GetAttribute("name").ToString());
			m_ClientConnection.MessageGrabber.Add(item.Jid, new BareJidComparer(), new MessageCB(MessageCallback), null);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="oSender">O sender.</param>
		/// <param name="_presence">Presence.</param>
		private void OnPresence(object oSender, Presence _presence)
		{
			//TODO: implement function


		}


		public void updatePresence(bool bIsVisible)
		{
			Presence presence = new Presence (ShowType.chat, "Online");
			presence.Type = PresenceType.available;

			if(bIsVisible == false)
			{
				presence = new Presence (ShowType.away, "away");
				presence.Type = PresenceType.invisible;
			}

			m_ClientConnection.Send(presence);
		}



		public void sendMessage(string sMessage, string sReceiverName)
		{
			//check if client has logged in successfully
			if (m_ClientConnection.XmppConnectionState != XmppConnectionState.SessionStarted) {
				Console.Write ("Connection not authenticated: " + m_ClientConnection.XmppConnectionState.ToString ());
				return;
			}

			//check if OTR-session is established
			if (m_OtrSessionManager.GetSessionMessageState (sReceiverName) != Mono.OTR.Utilities.OTR_MESSAGE_STATE.MSG_STATE_ENCRYPTED) {
				createOtrSession (sReceiverName);
			}

			//encrypt message if possible
			if (m_OtrSessionManager.GetSessionMessageState (sReceiverName) != Mono.OTR.Utilities.OTR_MESSAGE_STATE.MSG_STATE_ENCRYPTED) {
				Console.WriteLine ("Cannot send encrypted message. Encrypted session is not established. Session-State: " + m_OtrSessionManager.GetSessionMessageState (sReceiverName).ToString());
				return;
			}
			m_OtrSessionManager.EncryptMessage(sReceiverName, sMessage);

			//save message
			m_ConversationManager.addMessage ("xmpp", sMessage, m_JidSender, sReceiverName);
		}


		public bool createOtrSession(string sReceiverName)
		{
			try
			{
				if(m_OtrConnections.ContainsKey(sReceiverName) == false)
				{
					m_OtrSessionManager.CreateOTRSession (sReceiverName, m_OTRKeyRing.m_PrivateKey);
					m_OtrConnections.Add(sReceiverName, 0);
				}
			}
			catch(Exception e) {
				m_Logger.log(ELogLevel.LVL_WARNING, "Warning: " + e.Message, m_sModuleName);
				return false;
			}

			try
			{
				if(m_OtrConnections.ContainsKey(sReceiverName))
				{
					if(m_OtrConnections[sReceiverName] == 0)
					{
						m_OtrSessionManager.RequestOTRSession(sReceiverName, OTRSessionManager.GetSupportedOTRVersionList()[2]);
						m_OtrConnections[sReceiverName] = 1;
					}
				}
			}
			catch(Exception e) {
				m_Logger.log(ELogLevel.LVL_WARNING, "Warning: " + e.Message, m_sModuleName);
				return false;
			}
			return true;
		}


		public void startSMP()
		{
			//TODO:
//			m_OtrSessionManager.StartSMP (e.GetSessionID ());
//			m_OtrSessionManager.RequestExtraKeyUse (e.GetSessionID ());
//			m_OtrSessionManager.GetExtraSymmetricKey (e.GetSessionID ());
		}

		private void sendXmppMessage(string sMessage, string sReceiverName)
		{
			m_ClientConnection.Send(new agsXMPP.protocol.client.Message(new Jid(sReceiverName), agsXMPP.protocol.client.MessageType.chat, sMessage));
			m_Logger.log(ELogLevel.LVL_TRACE, "Sent xmpp message.", m_sModuleName);
		}



		private void MessageCallback(object sender, agsXMPP.protocol.client.Message sMessage, object oData)
		{
			if (sMessage.Body != null)
			{
				m_OtrSessionManager.ProcessOTRMessage (sMessage.From.Bare, sMessage.Body);
			}
		}



		private void handleMessage(object sender, agsXMPP.protocol.client.Message sMessage)
		{
			m_OtrSessionManager.ProcessOTRMessage (sMessage.From.Bare, sMessage.Body);
			m_Logger.log(ELogLevel.LVL_TRACE, "Processed otr message.", m_sModuleName);
			return;
		}



		private void OnOTREvent(object source, OTREventArgs e)
		{
			switch (e.GetOTREvent ()) {
				//gets fired if a message is received
				case OTR_EVENT.MESSAGE:
					//save received message using the conversation manager
					m_ConversationManager.addMessage ("xmpp", e.GetMessage (), e.GetSessionID (), m_JidSender);
					break;

				//gets fired if a message is ready to be sent
				case OTR_EVENT.SEND:
					sendXmppMessage (e.GetMessage (), e.GetSessionID ());
					break;


				case OTR_EVENT.ERROR:
					
					  Console.WriteLine(" OTR Error: {0} \n", e.GetErrorMessage());
					  Console.WriteLine(" OTR Error Verbose: {0} \n", e.GetErrorVerbose());

					break;


				case OTR_EVENT.READY:
					//TODO: implement store to know if an otr session is established
					
					break;
				case OTR_EVENT.DEBUG:
				m_Logger.log(ELogLevel.LVL_DEBUG, e.GetMessage(), m_sModuleName);
					break;
				case OTR_EVENT.EXTRA_KEY_REQUEST:
					m_OtrSessionManager.GetExtraSymmetricKey(e.GetSessionID());
					break;
				
				case OTR_EVENT.SMP_MESSAGE:
					Console.WriteLine ("SMP-Event: " + e.GetSMPEvent ().ToString ());
					//TODO: check message state
					break;

				case OTR_EVENT.CLOSED:
					//TODO: set session state to not otr encrypted
					break;
			}
		}

	}
}


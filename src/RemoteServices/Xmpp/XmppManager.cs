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
	public class XmppManager : IProtocolManager
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sJid">S jid.</param>
		/// <param name="sPassword">S password.</param>
		public XmppManager (string sJid, string sPassword)
		{
			m_JidSender = new Jid (sJid);
			m_ClientConnection = new XmppClientConnection(m_JidSender.Server);
			m_Password = sPassword;
			m_Contacts = new Dictionary<string, string> ();
			m_ClientConnection.Open(m_JidSender.User, m_Password);

			//register EventHandlers
			m_ClientConnection.OnLogin += new ObjectHandler(OnLogin);
			m_ClientConnection.OnRosterItem += new XmppClientConnection.RosterHandler(OnRosterItem);
			m_ClientConnection.OnPresence += new PresenceHandler(OnPresence);

			//info: message callback is registered in onRosterItem callback
		}

		public string m_sProtocolName {
			get { return "xmpp"; }
		}

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

		/// <summary>
		/// The m password stores the password for the loggedIn user.
		/// </summary>
		private string m_Password;

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
		private void OnLogin(object oSender)
		{
			m_IsLoggedIn = true;
			m_IsAuthenticated = m_ClientConnection.Authenticated;
			m_ConnectionState = m_ClientConnection.XmppConnectionState;
			m_Presence = new Presence (ShowType.chat, "Online");
			m_Presence.Type = PresenceType.available;
			m_ConversationManager = new ConversationManager ();
			m_ConversationManager.m_OwnJid = this.m_JidSender;
			m_ClientConnection.Send (m_Presence);
			m_OtrSessionManager = new OTRSessionManager(m_JidSender);
			m_OtrSessionManager.OnOTREvent += new OTREventHandler(OnOTREvent);
		}

		private void OnRosterItem(object sender, RosterItem item)
		{
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

		public void init()
		{

		}

		public void start()
		{

		}


		public void sendMessage(string sMessage, string sReceiverName)
		{
			m_ClientConnection.Send(new agsXMPP.protocol.client.Message(new Jid(sReceiverName), agsXMPP.protocol.client.MessageType.chat, sMessage));
		}


		private void MessageCallback(object sender, agsXMPP.protocol.client.Message sMessage, object oData)
		{
			if (sMessage.Body != null)
			{
				m_ConversationManager.addMessage (m_sProtocolName, sMessage.Body, sMessage.From, sMessage.To);
			}
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
					sendMessage (e.GetMessage (), e.GetSessionID ());
					break;


				case OTR_EVENT.ERROR:

					//  Console.WriteLine("Alice: OTR Error: {0} \n", e.GetErrorMessage());
					//  Console.WriteLine("Alice: OTR Error Verbose: {0} \n", e.GetErrorVerbose());

					break;


				case OTR_EVENT.READY:
					//TODO: implement store to know if an otr session is established
					//TODO: check if correct
					/*
					m_OtrSessionManager.StartSMP (e.GetSessionID ());
					m_OtrSessionManager.RequestExtraKeyUse (e.GetSessionID ());
					m_OtrSessionManager.GetExtraSymmetricKey (e.GetSessionID ());
					*/
					break;
				case OTR_EVENT.DEBUG:
					//Console.WriteLine("Alice: " + e.GetMessage() + "\n");
					break;
				case OTR_EVENT.EXTRA_KEY_REQUEST:
					m_OtrSessionManager.GetExtraSymmetricKey(e.GetSessionID());
					break;
				
				case OTR_EVENT.SMP_MESSAGE:
					//TODO: check message state
					break;
				case OTR_EVENT.CLOSED:
					//TODO: set session state to not otr encrypted
					break;
			}
		}








	}
}


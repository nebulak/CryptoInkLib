using System;
using System.Collections.Generic;
using agsXMPP;
using agsXMPP.protocol.client;
using agsXMPP.Collections;
using agsXMPP.protocol.iq.roster;
using System.Threading;


namespace CryptoInkLib
{
	/// <summary>
	/// The XmppManager class establishs and manages a connection to a Jabber-Server
	/// </summary>
	public class XmppManager
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
		}

		/// <summary>
		/// The m jid sender stores the JID for the loggedIn user.
		/// </summary>
		private Jid m_JidSender;

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
		private XmppConversationManager m_ConversationManager;

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
			m_ConversationManager = new XmppConversationManager ();
			m_ConversationManager.m_OwnJid = this.m_JidSender;
			m_ClientConnection.Send (m_Presence);
		}

		private void OnRosterItem(object sender, RosterItem item)
		{
			m_Contacts.Add(item.GetAttribute("jid").ToString(), item.GetAttribute("name").ToString());
			m_ClientConnection.MessageGrabber.Add(item.Jid, new BareJidComparer(), new MessageCB(MessageCallBack), null);
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


		public void sendMessage(string sMessage, string sReceiverName)
		{
			m_ClientConnection.Send(new agsXMPP.protocol.client.Message(new Jid(sReceiverName), agsXMPP.protocol.client.MessageType.chat, sMessage));
		}


		private void MessageCallBack(object sender, agsXMPP.protocol.client.Message sMessage, object oData)
		{
			if (sMessage.Body != null)
			{
				m_ConversationManager.addMessage (sMessage.Body, sMessage.From, sMessage.To);
			}
		}








	}
}


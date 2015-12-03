using System;
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
			m_ClientConnection.Open(m_JidSender.User, m_Password);

			//register EventHandlers
			m_ClientConnection.OnLogin += new ObjectHandler(handleOnLogin);
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
		/// The m roster stores the roster of the user.
		/// </summary>
		private Roster m_Roster;


		/// <summary>
		/// handles the OnLogin-event and updates the Presence of the user
		/// </summary>
		/// <param name="oSender">The object which sent the event.</param>
		static void handleOnLogin(object oSender)
		{
			m_IsLoggedIn = true;
			m_IsAuthenticated = m_ClientConnection.Authenticated;
			m_ConnectionState = m_ClientConnection.XmppConnectionState;
			m_Presence = new Presence (ShowType.chat, "Online");
			m_Presence.Type = PresenceType.available;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="oSender">O sender.</param>
		/// <param name="_presence">Presence.</param>
		static void handleOnPresence(object oSender, Presence _presence)
		{
			//TODO: implement function
		}









	}
}


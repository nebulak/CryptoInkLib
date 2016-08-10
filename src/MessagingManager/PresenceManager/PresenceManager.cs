using System;
using System.Collections.Generic;
using agsXMPP;
using agsXMPP.protocol.client;
using agsXMPP.Collections;

namespace CryptoInkLib
{
	public class PresenceManager
	{
		public PresenceManager (AddressBook addressBook)
		{
			m_AddressBook = addressBook;
		}
		public AddressBook m_AddressBook;
		public List<ContactPresence> m_PresenceList;


		//TODO: also check showType
		public void update(Presence presence)
		{
			string sJid = presence.From.Bare;
			string sText = presence.Status;
			bool isOnline = false;

			//handle PresenceType
			if ((presence.Type == PresenceType.available) || (presence.Type == PresenceType.invisible)) {
				isOnline = true;
			} else if (presence.Type == PresenceType.subscribe) {
				//TODO: handle subscribe request
			} else if (presence.Type == PresenceType.subscribed) {
				//TODO: handle answered subscribe message
			} else if (presence.Type == PresenceType.unavailable) {
				isOnline = false;
			}

			//update presenceList
			for (int i = 0; i < m_PresenceList.Count; i++) {
				if (m_PresenceList [i].sJid == sJid) {
					m_PresenceList.RemoveAt (i);
					break;
				}
			}

			//add ContactPresence if it does not exist
			ContactPresence newPresence = new ContactPresence ();
			newPresence.sJid = sJid;
			newPresence.bIsOnline = isOnline;
			newPresence.sText = sText;

			this.m_PresenceList.Add (newPresence);
		}

		public bool isContactOnline(string sJid)
		{
			for (int i = 0; i < m_PresenceList.Count; i++) {
				if (m_PresenceList [i].sJid == sJid) {
					return m_PresenceList [i].bIsOnline;
				}
			}
			return false;
		}
	}
}


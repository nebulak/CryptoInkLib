using System;
using System.Collections.Generic;

namespace CryptoInkLib
{
	public class ContactManager
	{
		public ContactManager ()
		{
			
		}

		public ContactManager (List<Contact> contactList)
		{
			this.m_Contacts = contactList;
		}

		public List<Contact> m_Contacts;



		public RC addContact(Contact newContact)
		{
			//check if contact already exists
			foreach(Contact _contact in m_Contacts)
			{
				if (_contact.id == newContact.id) {
					return RC.RC_CONTACT_ALREADY_EXISTS;
				}

				if (_contact.nickname == newContact.nickname) {
					return RC.RC_CONTACT_ALREADY_EXISTS;
				}
			}
			m_Contacts.Add (newContact);
			return 0;
		}


		public RC updateContact(Contact updatedContact, Contact originalContact)
		{
			if (m_Contacts.Contains (originalContact)) {
				bool rc = m_Contacts.Remove (originalContact);

				if (rc == false) {
					return RC.RC_CONTACT_COULD_NOT_BE_UPDATED;
				}
				m_Contacts.Add (updatedContact);

				return RC.RC_OK;
			}
			return RC.RC_CONTACT_COULD_NOT_BE_UPDATED;
		}


		public RC deleteContact(Contact contact)
		{
			bool rc = m_Contacts.Remove (contact);
			if (rc == false) {
				return RC.RC_CONTACT_COULD_NOT_BE_DELETED;
			}
			return RC.RC_OK;
		}


		public Contact getContactByNickname(string sNickname)
		{
			foreach (Contact contact in m_Contacts) {
				if (contact.nickname == sNickname) {
					return contact;
				}
			}
			//return empty contact
			return new Contact();
		}


		public Contact getContactById(string sId)
		{
			foreach (Contact contact in m_Contacts) {
				if (contact.id == sId) {
					return contact;
				}
			}
			return new Contact();
		}

		//TODO: delete
		/*public bool isProtocolSupportedByContact(Contact contact, ECommunicationProtocols eProtocol)
		{
			if (contact.supportedProtocols.Contains (eProtocol)) {
				return true;
			}
			return false;
		}*/
	}
}


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



		public int addContact(Contact newContact)
		{
			//check if contact already exists
			foreach(Contact _contact in m_Contacts)
			{
				if (_contact.sId == newContact.sId) {
					//TODO: return errorcode
					return 1;
				}

				if (_contact.sNickname == newContact.sNickname) {
					//TODO: return errorcode
					return 2;
				}
			}
			m_Contacts.Add (newContact);
			return 0;
		}


		public int updateContact(Contact updatedContact, Contact originalContact)
		{
			if (m_Contacts.Contains (originalContact)) {
				bool rc = m_Contacts.Remove (originalContact);

				if (rc == false) {
					//TODO: return error code
					return 2;
				}
				m_Contacts.Add (updatedContact);

				return 0;
			}
			//TODO: return error code
			return 2;
		}


		public int deleteContact(Contact contact)
		{
			bool rc = m_Contacts.Remove (contact);
			if (rc == false) {
				//TODO: return error code
				return 2;
			}
			return 0;
		}


		public Contact getContactByNickname(string sNickname)
		{
			foreach (Contact contact in m_Contacts) {
				if (contact.sNickname == sNickname) {
					return contact;
				}
			}
			//TODO: return null...
			return new Contact();
		}


		public Contact getContactById(string sId)
		{
			//TODO: implement
			return new Contact();
		}
	}
}


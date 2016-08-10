using System;

namespace CryptoInkLib
{
	public class CommandParserConfig
	{
		//TODO: use directories for key-management
		public AuthInfo authInfo;
		public EmailServiceDescription emailServiceDescription;
		public OpenPGPRing openPgpRing;
		public XmppServiceDescription xmppServiceDescription;
		public OTRKeyRing otrKeyring;
		public AddressBook addressBook;
		public Logger logger;
		public int inboxCheckIntervall;
		public bool isInMinutes;

		public CommandParserConfig ()
		{
		}
	}
}


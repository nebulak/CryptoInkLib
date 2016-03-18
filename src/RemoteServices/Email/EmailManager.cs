using System;
using MailKit.Net.Smtp;
using MailKit.Net.Pop3;
using MailKit.Net.Imap;
using MailKit;
using MimeKit;
using System.Collections.Generic;

namespace CryptoInkLib
{
	public class EmailManager
	{
		
		public EmailManager (AuthInfo authInfo, EmailServiceDescription serviceDescription, OpenPGPRing openPgpRing, ConversationManager conversationManager, Logger logger)
		{
			m_AuthInfo = authInfo;
			m_EmailServiceDescription = serviceDescription;
			m_OpenPGPRing = openPgpRing;
			m_OpenPgpCrypter = new OpenPgpCrypter (m_OpenPGPRing.m_PublicKeyRing, m_OpenPGPRing.m_PrivateKeyRing, m_OpenPGPRing.m_cPassword);
			m_Logger = logger;
			m_sProtocol = "email";
			m_sModuleName = "EmailManager";

		}


		public ConversationManager m_ConversationManager;
		public string m_sModuleName;
		private AuthInfo m_AuthInfo;
		private EmailServiceDescription m_EmailServiceDescription;
		private OpenPGPRing m_OpenPGPRing;
		private OpenPgpCrypter m_OpenPgpCrypter;
		private Logger m_Logger;
		private string m_sProtocol;


		public RC sendMessage(string sReceiverAddress, string sMessage, int iSmtpPort=587)
		{
			var message = new MimeMessage ();
			message.From.Add(new MailboxAddress(m_AuthInfo.m_sId, m_AuthInfo.m_sId));
			message.To.Add (new MailboxAddress (sReceiverAddress, sReceiverAddress));

			message.Subject = "";

			try
			{
				string sEncryptedMessage = m_OpenPgpCrypter.encryptPgpString (sMessage, sReceiverAddress, true, false);
			}
			catch(Exception e) {
				m_Logger.log (ELogLevel.LVL_WARNING, e.Message, m_sModuleName);
			}

			message.Body = new TextPart ("plain") {
				Text = @sMessage
			};

			using (var client = new SmtpClient ()) {
				client.Connect (m_EmailServiceDescription.SmtpUrl, iSmtpPort, true);

				// disable the XOAUTH2 authentication mechanism.
				client.AuthenticationMechanisms.Remove ("XOAUTH2");

				client.Authenticate (m_AuthInfo.m_sId, m_AuthInfo.m_sPassword);

				client.Send (message);
				client.Disconnect (true);

				this.m_ConversationManager.addMessage (m_sProtocol, sMessage, m_AuthInfo.m_sId, sReceiverAddress);
			}

			return RC.RC_OK;
		}



		public RC getMessages(bool bUsePop3 = false, int iPortToUse = 0)
		{
			if (bUsePop3) {
				using (var client = new Pop3Client ()) {
					
					if (iPortToUse == 0) {
						client.Connect (m_EmailServiceDescription.Pop3Url, 110, true);
					} else {
						client.Connect (m_EmailServiceDescription.Pop3Url, iPortToUse, true);
					}

					// Note: since we don't have an OAuth2 token, disable
					// the XOAUTH2 authentication mechanism.
					client.AuthenticationMechanisms.Remove ("XOAUTH2");

					client.Authenticate (m_AuthInfo.m_sId, m_AuthInfo.m_sPassword);

					for (int i = 0; i < client.Count; i++) {
						var message = client.GetMessage (i);
						string sPlainBody = m_OpenPgpCrypter.DecryptPgpString(message.GetTextBody(MimeKit.Text.TextFormat.Text));
						m_ConversationManager.addMessage (m_sProtocol, message.Subject + " " + sPlainBody, message.Sender.Address, m_AuthInfo.m_sId);
					}

					client.Disconnect (true);
					return RC.RC_OK;
				}
				return RC.RC_INBOX_NOT_AVAILABLE;
			}
			else //use IMAP 
			{
				using (var client = new ImapClient ()) {

					if (iPortToUse == 0) {
						client.Connect (m_EmailServiceDescription.ImapUrl, 993, true);
					} else {
						client.Connect (m_EmailServiceDescription.ImapUrl, iPortToUse, true);
					}

					// Note: since we don't have an OAuth2 token, disable
					// the XOAUTH2 authentication mechanism.
					client.AuthenticationMechanisms.Remove ("XOAUTH2");

					client.Authenticate (m_AuthInfo.m_sId, m_AuthInfo.m_sPassword);

					// The Inbox folder is always available on all IMAP servers...
					var inbox = client.Inbox;
					inbox.Open (FolderAccess.ReadOnly);

					Console.WriteLine ("Total messages: {0}", inbox.Count);
					Console.WriteLine ("Recent messages: {0}", inbox.Recent);

					for (int i = 0; i < inbox.Count; i++) {
						var message = inbox.GetMessage (i);
						m_ConversationManager.addMessage (m_sProtocol, message.Subject + message.Body, message.Sender.Address, m_AuthInfo.m_sId);
					}

					client.Disconnect (true);
					return RC.RC_OK;
				}
			}


		}
	}
}


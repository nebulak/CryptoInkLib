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
		//TODO: get config values from dictionary
		public EmailManager (Dictionary<string, string> dConfig, string sEmailAddress, string sPassword, ConversationManager _conversationManager)
		{
			this.m_ConversationManager = _conversationManager;
			this.m_sEmailAddress = sEmailAddress;
			this.m_sPassword = sPassword;
		}


		public EmailManager (Dictionary<string, string> dConfig, string sEmailAddress, string sPassword, string sSmtpServerUrl, string sPop3Url, string sImapUrl)
		{
			this.m_sEmailAddress = sEmailAddress;
			this.m_sPassword = sPassword;
			this.m_sSmtpServerUrl = sSmtpServerUrl;
			this.m_sPop3ServerUrl = sPop3Url;
			this.m_sImapServerUrl = sImapUrl;
		}


		public ConversationManager m_ConversationManager;
		public string m_sEmailAddress;
		private string m_sPassword;
		private string m_sSmtpServerUrl;
		private string m_sPop3ServerUrl;
		private string m_sImapServerUrl;


		//TODO: add function which can also add files to email
		public int sendMessage(string sReceiverAddress, string sMessage, string sSubject="")
		{
			var message = new MimeMessage ();
			//TODO: What name to use? should we really use the email-address as the name??
			message.From.Add(new MailboxAddress(m_sEmailAddress, m_sEmailAddress));
			message.To.Add (new MailboxAddress (sReceiverAddress, sReceiverAddress));

			message.Subject = sSubject;

			message.Body = new TextPart ("plain") {
				Text = @sMessage
			};

			using (var client = new SmtpClient ()) {
				//TODO: make Port and SSL configurable
				client.Connect (this.m_sSmtpServerUrl, 587, false);

				// Note: since we don't have an OAuth2 token, disable
				// the XOAUTH2 authentication mechanism.
				client.AuthenticationMechanisms.Remove ("XOAUTH2");

				// Note: only needed if the SMTP server requires authentication
				client.Authenticate (this.m_sEmailAddress, this.m_sPassword);

				client.Send (message);
				client.Disconnect (true);

				this.m_ConversationManager.addMessage ("email", sMessage, this.m_sEmailAddress, sReceiverAddress);
			}

			return 0;
		}

		//TODO: what type should be returned here ?
		public void getMessages(bool bUsePop3 = false)
		{
			if (bUsePop3) {
				using (var client = new Pop3Client ()) {
					//TODO: make Port and SSL configurable
					client.Connect (this.m_sPop3ServerUrl, 110, false);

					// Note: since we don't have an OAuth2 token, disable
					// the XOAUTH2 authentication mechanism.
					client.AuthenticationMechanisms.Remove ("XOAUTH2");

					client.Authenticate (this.m_sEmailAddress, this.m_sPassword);

					for (int i = 0; i < client.Count; i++) {
						var message = client.GetMessage (i);
						Console.WriteLine ("Subject: {0}", message.Subject);
					}

					client.Disconnect (true);
				}
			}
			else //use IMAP 
			{
				using (var client = new ImapClient ()) {
					//TODO: make Port and SSL configurable
					client.Connect (this.m_sImapServerUrl, 993, true);

					// Note: since we don't have an OAuth2 token, disable
					// the XOAUTH2 authentication mechanism.
					client.AuthenticationMechanisms.Remove ("XOAUTH2");

					client.Authenticate (this.m_sEmailAddress, this.m_sPassword);

					// The Inbox folder is always available on all IMAP servers...
					var inbox = client.Inbox;
					inbox.Open (FolderAccess.ReadOnly);

					Console.WriteLine ("Total messages: {0}", inbox.Count);
					Console.WriteLine ("Recent messages: {0}", inbox.Recent);

					for (int i = 0; i < inbox.Count; i++) {
						var message = inbox.GetMessage (i);
						Console.WriteLine ("Subject: {0}", message.Subject);
					}

					client.Disconnect (true);
				}

			}


		}
	}
}


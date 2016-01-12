using System;

namespace CryptoInkLib
{
	public interface IProtocolManager
	{
		bool m_isInstantMessageProtocol {get;}
		//TODO: m_sProtocolName is defined as static string in XmppManager. What to do here?
		//string m_sProtocolName{ get;}
		void init();
		void start();
		void sendMessage(string sMessage, string sReceiverName);
	}
}


using System;

namespace CryptoInkLib
{
	public interface IProtocolManager
	{
		bool m_isInstantMessageProtocol {get;}
		string m_sProtocolName{ get;}
		void init();
		void start();
		void sendMessage(string sMessage, string sReceiverName);
	}
}


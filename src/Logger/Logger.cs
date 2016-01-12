using System;
using System.Collections.Generic;

namespace CryptoInkLib
{
	public class Logger
	{
		

		public Logger ()
		{
			m_sLogType = "memory";
		}

		public string m_sLogType;
		public ELogLevel m_LogLevel;
		public List<string> m_LogList;


		public void log(ELogLevel eLogLevel, string sMessage, string sModule)
		{
			//check if LogLevel is high enough to be logged
			if (eLogLevel < m_LogLevel) {
				return;
			}

			switch (m_sLogType) {
			case "memory":
				m_LogList.Add (createLogEntry (eLogLevel, sMessage, sModule));
				break;
			default:
				m_LogList.Add (createLogEntry (eLogLevel, sMessage, sModule));
				break;
			}
		}


		private string createLogEntry(ELogLevel eLogLevel, string sMessage, string sModule)
		{
			return eLogLevel.ToString () + " | " + sModule + " | " + sMessage;
		}
	}
}


using System;

namespace CryptoInkLib
{
	public class Logger
	{
		public static int LVL_TRACE = 0;
		public static int LVL_DEBUG = 1;
		public static int LVL_INFO = 2;
		public static int LVL_WARNING = 3;
		public static int LVL_ERROR = 4;

		public Logger ()
		{
			m_sLogType = "memory";
		}

		public string m_sLogType;
		public int m_iLogLevel;
		public List<string> m_LogList


		public void log(int iLogLevel, string sMessage, string sModule)
		{
			//check if LogLevel is high enough to be logged
			if (iLogLevel < m_iLogLevel) {
				return;
			}



		}
	}
}


using System;

namespace CryptoInkLib
{
	public class AuthInfo
	{
		public AuthInfo (	EAuthType preferredAuthType, 
							string sId, 
							string sPassword,
							string sToken,
							byte[] bCertificate
		)
		{
			m_preferredAuthType = preferredAuthType;
			m_sId = sId;
			m_sPassword = sPassword;
			m_sToken = sToken;
			m_bCertificate = bCertificate;
		}

		public EAuthType m_preferredAuthType;
		public string m_sId;
		public string m_sPassword;
		public string m_sToken;
		public byte[] m_bCertificate;
	}
}


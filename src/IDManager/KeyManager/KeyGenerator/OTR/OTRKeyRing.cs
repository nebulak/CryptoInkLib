using System;
using System.Collections.Generic;
using Mono.OTR.Interface;
using Mono.OTR.Utilities;

namespace CryptoInkLib
{
	public class OTRKeyRing
	{
		public OTRKeyRing ()
		{
			
		}

		public DSAKeyParams m_PrivateKey;
		public List<OTRFingerprint> m_FingerprintList;

		public string getFingerprint(string sJid)
		{
			foreach (OTRFingerprint fingerprint in m_FingerprintList) {
				if (fingerprint.m_sOwnerId == sJid) {
					return fingerprint.m_sFingerprint;
				}
			}
			return "";
		}
	}
}


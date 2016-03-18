using System;
using Mono.OTR.Interface;
using Mono.OTR.Utilities;

namespace CryptoInkLib
{
	public class OTRKeyRingGenerator
	{
		public OTRKeyRingGenerator ()
		{
		}

		public static OTRKeyRing createOTRKeyRing(string sID)
		{
			OTRKeyRing otrKeyring = new OTRKeyRing ();

			//create private DSA Key parameters
			DSASigner dsaSigner = new DSASigner ();
			otrKeyring.m_PrivateKey = dsaSigner.GetDSAKeyParameters ();

			//create fingerprint
			OTRFingerprint fingerprint = new OTRFingerprint ();
			fingerprint.m_sOwnerId = sID;
			fingerprint.m_sFingerprint = dsaSigner.GetDSAPublicKeyFingerPrintHex ();

			//add fingerprint to keyring
			otrKeyring.m_FingerprintList.Add(fingerprint);

			return otrKeyring;
		}

	}
}


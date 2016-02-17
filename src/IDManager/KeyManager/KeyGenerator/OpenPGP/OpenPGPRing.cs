using Org.BouncyCastle.Bcpg;
using Org.BouncyCastle.Bcpg.OpenPgp;
using Org.BouncyCastle.Bcpg.Sig;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using System;
using System.Collections;
using System.IO;

namespace CryptoInkLib
{
	public class OpenPGPRing
	{
		public PgpPublicKeyRing m_PublicKeyRing;
		public PgpSecretKeyRing m_PrivateKeyRing;
		public char[] m_cPassword;

		public OpenPGPRing (PgpPublicKeyRing _pgpPublicKeyRing, PgpSecretKeyRing _pgpSecretKeyRing, char [] cPassword)
		{
			this.m_PublicKeyRing = _pgpPublicKeyRing;
			this.m_PrivateKeyRing = _pgpSecretKeyRing;
			this.m_cPassword = cPassword;
		}
	}
}


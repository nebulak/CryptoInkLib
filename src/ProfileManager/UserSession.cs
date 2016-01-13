using System;

namespace CryptoInkLib
{
	public class UserSession
	{
		public UserSession ()
		{

		}

		public string 			m_sKeyStorePath		{ get; set; }
		public byte [] 			m_baPasswordKey		{ get; set; }
		public byte [] 			m_baPasswordKeySalt { get; set; }
		public byte [] 			m_baStorage_IV 		{ get; set; }
		public KeyStoreStorage 	m_KeyStoreStorage	{ get; set; }

		public KeyStore toKeyStore()
		{
			KeyStore _KeyStore = new KeyStore ();

			_KeyStore.PasswordKeySalt 	= this.m_baPasswordKeySalt;
			_KeyStore.StorageIV 		= this.m_baStorage_IV;
			_KeyStore.Storage 			= KeyStoreCrypto.encryptKeyStoreStorage (this.m_baPasswordKey, this.m_baStorage_IV, this.m_KeyStoreStorage);

			return _KeyStore;
		}


	}
}


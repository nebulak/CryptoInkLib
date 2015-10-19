using System;
using System.IO;
using System.Text;
using System.Reflection;
using Newtonsoft.Json;
using Org.BouncyCastle.Crypto; //PBEParametersGenerator;
using Org.BouncyCastle.Crypto.Generators; //PKCS5S2ParametersGenerator;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.X509;


using Org.BouncyCastle.Crypto.Digests; //SHA3Digest;
using Org.BouncyCastle.Crypto.Parameters; //.KeyParameter;
using Org.BouncyCastle.Crypto.Prng; //DigestRandomGenerator;


//for aes encryption
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Paddings;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.Crypto.IO;
using Org.BouncyCastle.Utilities.IO.Pem;

//for HMAC
using Org.BouncyCastle.Crypto.Macs;
using Org.BouncyCastle.Math;

namespace CryptoInkLib
{

	public class KeyStoreManager
	{

		public KeyStoreManager ()
		{

		}



		public int createUserKeyStore(string c_sPassword, string c_sKeyStoreName, string c_sKeyStorePath)
		{
			/*
			 * Generate and create keys
			*/

			//create and encode userKey
			var userKeyPair 		= KeyStoreCrypto.generateUserKeyPair (4096);
			var encodedPrivate 		= KeyStoreCrypto.encodeUserKeyPairPrivate (userKeyPair);
			var serializedPublic 	= KeyStoreCrypto.encodeUserKeyPairPublic (userKeyPair);

			//create Random Number Generator
			var rng = new System.Security.Cryptography.RNGCryptoServiceProvider();

			//create passwordKey from password
			byte[] salt = new byte[16];
			rng.GetBytes (salt);

			byte[] passwordKey = KeyStoreCrypto.createPasswordKey (c_sPassword, salt);


			/*
			 * Encrypt keys and add them to keystore
			*/

			//create Keystore and add Header
			KeyStore keyStore = new KeyStore ();
			keyStore.PasswordKeySalt = salt;

			//create userKey Object
			UserKey userKey = createUserKey (serializedPublic, encodedPrivate);

			//create KeyStoreStorage and add our first userKey
			KeyStoreStorage keyStoreStorage = new KeyStoreStorage();
			keyStoreStorage.userKeys = new [] { userKey };
			keyStoreStorage.friendUserKeys = null;


			//get IV for storage encryption
			byte[] storage_iv 	= new byte[16];

			rng.GetBytes (storage_iv);
			keyStore.StorageIV = storage_iv;

			//Encrypt keyStoreStorage
			keyStore.Storage = KeyStoreCrypto.encryptKeyStoreStorage (passwordKey, storage_iv, keyStoreStorage);

			//Get KeyStore as String
			string jsonKeystore = JsonConvert.SerializeObject (keyStore);

			//Write Keystore to file
			string sAppPath = System.IO.Path.Combine(c_sKeyStorePath, (c_sKeyStoreName + ".keystore"));
			File.WriteAllText(sAppPath, jsonKeystore);


			return 0;
		}



		public UserSession loadUserKeyStore(string c_sPassword, string c_sKeyStoreName, string c_sKeyStorePath)
		{
			//read contents from KeyStore-file
			StreamReader _StreamReader 	= new StreamReader (Path.Combine(c_sKeyStorePath, c_sKeyStoreName));
			string sKeyStoreContent 	= _StreamReader.ReadToEnd();
			_StreamReader.Close();


//			//create PasswordKey from Password
			KeyStore _KeyStore 			= JsonConvert.DeserializeObject <KeyStore> (sKeyStoreContent);
			byte [] baPasswordKey 		= KeyStoreCrypto.createPasswordKey (c_sPassword, _KeyStore.PasswordKeySalt);


			KeyStoreStorage _KeyStoreStorage = KeyStoreCrypto.decryptKeyStoreStorage (baPasswordKey, _KeyStore.StorageIV, _KeyStore.Storage);

			if (_KeyStoreStorage == null) {
				return null;
			}
			//Create UserSession
			UserSession _UserSession 			= new UserSession ();
			_UserSession.m_sKeyStorePath 		= Path.Combine(c_sKeyStorePath, c_sKeyStoreName);
			_UserSession.m_baPasswordKey 		= baPasswordKey;
			_UserSession.m_baPasswordKeySalt 	= _KeyStore.PasswordKeySalt;
			_UserSession.m_baStorage_IV 		= _KeyStore.StorageIV;
			_UserSession.m_KeyStoreStorage 		= _KeyStoreStorage;

			return _UserSession;
		}



		public int updateKeyStore(UserSession c_UserSession, string c_sPassword)
		{
			//transform UserSession to KeyStore
			KeyStore _KeyStore = c_UserSession.toKeyStore();

			//TODO: changed to AES-GCM, check if return value is null to check the validity
			//check password validity
			if( ! KeyStoreCrypto.isPasswordValid(c_sPassword, c_UserSession.m_baPasswordKeySalt, c_UserSession.m_baPasswordKey))
			{
				return 1;
			}

			//create KeyStore as String
			string sJsonKeyStore = JsonConvert.SerializeObject (_KeyStore);


			return 0;
		}


		public UserKey createUserKey(byte[] serializedPublic, byte[] encodedPrivate)
		{
			UserKey userKey = new UserKey ();
			userKey.publicKey = Convert.ToBase64String (serializedPublic);
			userKey.privateKey = Convert.ToBase64String (encodedPrivate);
			userKey.status = 1;
			userKey.creationDate = DateTime.Now.ToString("d/M/yyyy");
			userKey.revokationDate = null;

			return userKey;
		}


		public KeyStore changeKeyStorePassword(string c_sOldPassword, string c_sNewPassword, string c_sKeyStoreName, string c_sKeyStorePath)
		{
			//TODO:
			// - load keystore with password
			// - change UserSession Keys
			// - transform UserSession to KeyStore
			// - save new KeyStore
			KeyStore _KeyStore = new KeyStore ();
			return _KeyStore;
		}


			
	}
}


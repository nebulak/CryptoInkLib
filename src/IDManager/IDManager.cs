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

	public class IDManager
	{

		public IDManager ()
		{

		}





		public RC createID(string c_sPassword, string c_sIDName, string c_sIDPath, string sProviderDomain, string sServiceLevelName)
		{
			AccountApi accountApi = new AccountApi (sProviderDomain);

			//check if service level exists
			ProviderInfo providerInfo = accountApi.getProviderInfo ();
			bool bDoesServiceLevelExist = false;
			foreach (ServiceLevel level in providerInfo.levels) {
				if (level.name == sServiceLevelName) {
					bDoesServiceLevelExist = true;
					break;
				}
			}
			if (bDoesServiceLevelExist == false) {
				return RC.RC_SERVICE_LEVEL_NOT_AVAILABLE;
			}

			//TODO: create random password for provider
			string sProviderPassword = CommonCrypto.getRandomString(36);


			//TODO: use isUsernameAvailable to check username at provider...
			SignupResponse signupResponse = accountApi.signup(c_sIDName, sProviderPassword, sServiceLevelName);
			if (signupResponse.rc != 0) {
				//TODO: check for errors
			}


			/*
			 * Generate and create keys
			*/

			//create Random Number Generator
			var rng = new System.Security.Cryptography.RNGCryptoServiceProvider();

			//create passwordKey from password
			byte[] salt = new byte[16];
			rng.GetBytes (salt);

			//TODO: check password based key derivation algorithm/ use scrypt instead?
			byte[] passwordKey = IDCrypto.createPasswordKey (c_sPassword, salt);


			/*
			 * Encrypt keys and add them to keystore
			*/

			//create Keystore and add Header
			ID id = new ID ();
			id.PasswordKeySalt = salt;

			//create KeyStoreStorage and add keys
			IDStorage idStorage = new IDStorage();

			//create OpenPGP Keyring
			OpenPGPRing pgpRing =  OpenPGPKeyRingGenerator.generateKeyRing(c_sIDName, c_sPassword);
			Keyring pgpKeyRing = new Keyring ();
			pgpKeyRing.keyringContent = pgpRing;
			pgpKeyRing.keyringType = EKeyringType.OPEN_PGP;
			idStorage.keyBox.Add (pgpKeyRing);


			//TODO: create OTR Keyring
			OTRKeyRing otrRing = OTRKeyRingGenerator.generateKeyRing();
			Keyring otrKeyRing = new Keyring ();
			otrKeyRing.keyringContent = otrRing;
			otrKeyRing.keyringType = EKeyringType.OTR;

			//get IV for storage encryption
			byte[] storage_iv 	= new byte[16];

			rng.GetBytes (storage_iv);
			id.StorageIV = storage_iv;

			//Encrypt keyStoreStorage
			id.Storage = IDCrypto.encryptIdStorage (passwordKey, storage_iv, idStorage);

			//Get KeyStore as String
			string jsonKeystore = JsonConvert.SerializeObject (id);

			//Write Keystore to file
			string sAppPath = Path.Combine(c_sIDPath, (c_sIDName + ".keystore"));
			File.WriteAllText(sAppPath, jsonKeystore);


			return 0;
		}



		public UserSession loadUserKeyStore(string c_sPassword, string c_sKeyStoreName, string c_sKeyStorePath)
		{
			//read contents from KeyStore-file
			StreamReader _StreamReader 	= new StreamReader (Path.Combine(c_sKeyStorePath, c_sKeyStoreName));
			string sKeyStoreContent 	= _StreamReader.ReadToEnd();
			_StreamReader.Close();


			//create PasswordKey from Password
			ID _KeyStore 			= JsonConvert.DeserializeObject <ID> (sKeyStoreContent);
			byte [] baPasswordKey 		= IDCrypto.createPasswordKey (c_sPassword, _KeyStore.PasswordKeySalt);

			 
			IDStorage _KeyStoreStorage = IDCrypto.decryptKeyStoreStorage (baPasswordKey, _KeyStore.StorageIV, _KeyStore.Storage);

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
			ID _KeyStore = c_UserSession.toKeyStore();

			//TODO: changed to AES-GCM, check if return value is null to check the validity
			//check password validity
			if( ! IDCrypto.isPasswordValid(c_sPassword, c_UserSession.m_baPasswordKeySalt, c_UserSession.m_baPasswordKey))
			{
				return 1;
			}

			//create KeyStore as String
			string sJsonKeyStore = JsonConvert.SerializeObject (_KeyStore);


			return 0;
		}

		/*
		public FileEncUserKey createFileEncUserKey(byte[] serializedPublic, byte[] encodedPrivate)
		{
			FileEncUserKey fileEncKey = new FileEncUserKey ();
			fileEncKey.publicKey = Convert.ToBase64String (serializedPublic);
			fileEncKey.privateKey = Convert.ToBase64String (encodedPrivate);
			fileEncKey.status = 1;
			fileEncKey.creationDate = DateTime.Now.ToString("d/M/yyyy");
			fileEncKey.revocationDate = null;

			return fileEncKey;
		}*/


		public ID changeKeyStorePassword(string c_sOldPassword, string c_sNewPassword, string c_sKeyStoreName, string c_sKeyStorePath)
		{
			//TODO:
			// - load keystore with password
			// - change UserSession Keys
			// - transform UserSession to KeyStore
			// - save new KeyStore
			ID _KeyStore = new ID ();
			return _KeyStore;
		}


		private int registerAccount(string c_sIdName)
		{
			//TODO: register an account at the provider
			return 0;

		}
			
	}
}


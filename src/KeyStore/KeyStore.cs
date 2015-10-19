using System;

namespace CryptoInkLib
{
	/// <summary>
	/// Represents a KeyStore with it's salt for the password-key, the storage-iv and the encrypted storage of the keystore
	/// </summary>
	public class KeyStore
	{
		public KeyStore ()
		{

		}

		public byte[] PasswordKeySalt 	{ get; set; }
		public byte[] StorageIV 		{ get; set; }
		public string Storage 			{ get; set; } //storage is AES GCM encrypted

		public UserSession toUserSession(string c_sPassword, string c_sKeyStoreName, string c_sKeyStorePath)
		{
			UserSession _UserSession = new UserSession ();
			//TODO: create transformation

			return _UserSession;
		}
	}
}


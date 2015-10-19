using System;

namespace CryptoInkLib
{
	public class FileHeaderCrypter
	{
		const string HEADER_START = "CInk:";
		const string HEADER_END = ":CInk";
		const string CIPHER_V1 = "1";


		public FileHeaderCrypter ()
		{
		}
		//TODO:
		public string createFileHeader(byte [] baContentIV, byte[] baContentKey, byte [] baFileNameIV, byte[] baFileNameKey, FriendUserKey[] recipientKeyArray)
		{
			string sFileHeader = HEADER_START;

			//create FileHeader-object and add content- and filename-IV
			CInkFileHeader FileHeader = new CInkFileHeader ();
			FileHeader.content_iv = baContentIV;
			FileHeader.filename_iv = baFileNameIV;

			//create encrypted session-key-array for encrypted content-keys
			int iNumberOfRecipients = recipientKeyArray.Length;
			for (int i = 0; i < iNumberOfRecipients; i++) {
				//encrypt session-key for current recipient
				EncryptedSessionKey currentSessionKey 	= new EncryptedSessionKey ();
				currentSessionKey.id 					= recipientKeyArray [i].keyID;
				//TODO: encrypt the content key asymmetrically
				//currentSessionKey.sessionKey 			= asymEncrypt(baContentKey, recipientKeyArray[i].publicKey);
				FileHeader.content_keys[i] 				= currentSessionKey;

				//encrypt filename-key for current recipient
				EncryptedSessionKey currentFileNameKey 	= new EncryptedSessionKey ();
				currentFileNameKey.id 					= recipientKeyArray [i].keyID;

			}


			//create encrypted session-key-array for encrypted filename-keys


			//TODO: return created header
			return sFileHeader;

		}
	}
}


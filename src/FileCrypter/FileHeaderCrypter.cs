using System;
using Newtonsoft.Json;

namespace CryptoInkLib
{
	public class FileHeaderCrypter
	{
		public const string HEADER_START = "CInk:";
		public const string HEADER_END = ":CInk";
		public const string CIPHER_V1 = "1:";


		public FileHeaderCrypter ()
		{
		}



		public static string createFileHeader(byte [] baContentIV, byte[] baContentKey, byte [] baFileNameIV, byte[] baFileNameKey, FileEncUserKey[] recipientKeyArray)
		{
			string sFileHeader = HEADER_START + CIPHER_V1;

			//create FileHeader-object and add content- and filename-IV
			CInkFileHeader FileHeader = new CInkFileHeader ();
			FileHeader.content_iv = baContentIV;
			FileHeader.filename_iv = baFileNameIV;


			//create encrypted session-key-array for encrypted content-keys
			int iNumberOfRecipients = recipientKeyArray.Length;
			for (int i = 0; i < iNumberOfRecipients; i++) {
				//encrypt session-key for current recipient
				EncryptedSessionKey currentSessionKey 	= new EncryptedSessionKey ();
				currentSessionKey.id 					= recipientKeyArray[i].keyID;

				//encrypt the content key asymmetrically
				currentSessionKey.sessionKey 			= CommonCrypto.RsaEncrypt(recipientKeyArray[i].publicKey, baContentKey);
				FileHeader.content_keys [i] 			= currentSessionKey;

				//encrypt filename-key for current recipient
				EncryptedSessionKey currentFileNameKey 	= new EncryptedSessionKey ();
				currentFileNameKey.id 					= recipientKeyArray[i].keyID;
				currentFileNameKey.sessionKey			= CommonCrypto.RsaEncrypt(recipientKeyArray[i].publicKey, baFileNameKey);
				FileHeader.filename_keys [i] 			= currentFileNameKey;

			}

			//create fileHeader
			sFileHeader = sFileHeader + JsonConvert.SerializeObject (FileHeader) + HEADER_END;
			return sFileHeader;
		}



		public static CInkPlainHeader getDecryptedHeader(string sFileHeader, FileEncUserKey userKey)
		{
			
			string sFileHeaderWithoutStartString = sFileHeader.Substring(HEADER_START.Length + CIPHER_V1.Length);
			string sfileHeaderCore = sFileHeaderWithoutStartString.Substring (-5);
			CInkFileHeader oFileHeader = JsonConvert.DeserializeObject <CInkFileHeader> (sfileHeaderCore);

			CInkPlainHeader plainFileHeader = new CInkPlainHeader ();
			plainFileHeader.content_iv = oFileHeader.content_iv;
			plainFileHeader.filename_iv = oFileHeader.filename_iv;

			for (int i = 0; i < oFileHeader.content_keys.Length; i++) 
			{
				if (oFileHeader.content_keys [i].id == userKey.keyID) 
				{
					plainFileHeader.content_keys[0].sessionKey = CommonCrypto.RsaDecrypt (userKey.privateKey, oFileHeader.content_keys [i].sessionKey);
					plainFileHeader.filename_keys[0].sessionKey = CommonCrypto.RsaDecrypt (userKey.privateKey, oFileHeader.filename_keys [i].sessionKey);

					return plainFileHeader;
				}
			}
			return null;
		}

	}
}


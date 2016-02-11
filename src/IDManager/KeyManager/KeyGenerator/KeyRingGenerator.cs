using System;

namespace CryptoInkLib
{
	public class KeyGenerator
	{
		public KeyGenerator ()
		{
		}

		public static Key generate(string sKeyType, string sKeyID, int iStatus, SService sService)
		{
			if (sKeyType == "FileEncUserKey") 
			{
				Key _key = new Key ();
				_key.keyID = sKeyID;
				_key.type = sKeyType;
				_key.status = iStatus;
				_key.keyContent = FileEncUserKeyGen.generate ();
				_key.service = sService;

				return _key;
			}

			if (sKeyType == "OTRKey") 
			{

			}

			if (sKeyType == "OpenPGPKey") 
			{

			}

			return null;
		}
	}
}


using System;

namespace CryptoInkLib
{
	public class LoginResponse
	{
		public LoginResponse ()
		{
		}

		public int rc;
		public int error_description;
		public string auth_token;
	}
}


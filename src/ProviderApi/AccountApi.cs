using System;
using System.Collections.Generic;
using RestSharp;
using Newtonsoft.Json;

namespace CryptoInkLib
{
	public class AccountApi
	{
		public AccountApi (string sDomain)
		{
			m_sDomain = sDomain;
		}

		public string m_sDomain;
		public string m_sApiBase;
		private string m_sAuthToken;


		public ProviderInfo getProviderInfo()
		{
			var client = new RestClient(m_sDomain);
			var request = new RestRequest("api", Method.GET);

			// execute the request
			IRestResponse response = client.Execute(request);
			var content = response.Content;

			ProviderInfo info = new ProviderInfo ();
			info = JsonConvert.DeserializeObject<ProviderInfo>(content);

			m_sApiBase = info.api_uri + "/" + info.api_version.ToString ();

			return info;
		}
			

		public SignupResponse signup(string sUsername, string sPassword, string sServiceLevel)
		{
			var client = new RestClient(m_sApiBase);
			var request = new RestRequest("signup", Method.POST);
			request.AddParameter ("username", sUsername);
			request.AddParameter ("password", sPassword);
			request.AddParameter ("service_level", sServiceLevel);


			// execute the request
			IRestResponse response = client.Execute(request);
			var content = response.Content;

			SignupResponse apiResponse = JsonConvert.DeserializeObject<SignupResponse> (content);
			return apiResponse;
		}

		public bool isUsernameAvailable(string sUsername)
		{
			var client = new RestClient(m_sApiBase);
			var request = new RestRequest("username_available", Method.GET);
			request.AddParameter ("username", sUsername);

			// execute the request
			IRestResponse response = client.Execute(request);
			var content = response.Content;

			IsUsernameAvailableResponse apiResponse = JsonConvert.DeserializeObject<IsUsernameAvailableResponse> (content);

			return apiResponse.is_available;
		}



		public RC login(string sUsername, string sPassword)
		{
			var client = new RestClient(m_sApiBase);
			var request = new RestRequest("login", Method.POST);
			request.AddParameter ("username", sUsername);
			request.AddParameter ("password", sPassword);


			// execute the request
			IRestResponse response = client.Execute(request);
			var content = response.Content;

			LoginResponse apiResponse = JsonConvert.DeserializeObject<LoginResponse> (content);

			if (apiResponse.rc != 0) {
				return RC.RC_WRONG_PW;
			}

			m_sAuthToken = apiResponse.auth_token;
			return RC.RC_OK;
		}


		public AccountInfoResponse getAccountInfo()
		{
			var client = new RestClient(m_sApiBase);
			var request = new RestRequest("me", Method.GET);
			request.AddHeader ("Authentication", m_sAuthToken);

			// execute the request
			IRestResponse response = client.Execute(request);
			var content = response.Content;

			AccountInfoResponse apiResponse = JsonConvert.DeserializeObject<AccountInfoResponse> (content);
			return apiResponse;
		}


		//TODO: get info about configs for services
//		public Dictionary<string, string> getProviderConfigs()
//		{
//			var client = new RestClient(m_sApiBase);
//			var request = new RestRequest("configs", Method.GET);
//
//			// execute the request
//			IRestResponse response = client.Execute(request);
//			var content = response.Content;
//
//			Dictionary<string, string> apiResponse = JsonConvert.DeserializeObject<Dictionary<string, string>> (content);
//
//			//TODO: get keys of dictionary and load all configs from given urls
//			foreach (string entry in apiResponse) {
//				
//			}
//		}


	}
}


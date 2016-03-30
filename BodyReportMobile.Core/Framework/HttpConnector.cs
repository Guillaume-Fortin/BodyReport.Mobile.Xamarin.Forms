using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;
using MvvmCross.Platform;
using MvvmCross.Plugins.Messenger;

namespace BodyReportMobile.Core
{
	public class HttpConnector
	{
		private string _baseUrl = "http://localhost:5000/";
		private const string _relativeLoginUrl = "Api/Account/Login";
		private HttpClient _httpClient = null;
		private bool _connected = false;

		private string _userName = string.Empty;
		private string _password = string.Empty;

		private static HttpConnector _instance = null;

		public static HttpConnector Instance {
			get {
				if(_instance == null)
					_instance = new HttpConnector ();
				return _instance;
			}
		}

		private HttpConnector ()
		{
			//Now we make the same request with the token received by the auth service.
			CookieContainer cookies = new CookieContainer();
			HttpClientHandler handler = new HttpClientHandler();
			handler.CookieContainer = cookies;

			_httpClient = new System.Net.Http.HttpClient (handler);
		}

		public async Task<bool> ConnectUser(string userName, string password)
		{
			_userName = userName;
			_password = password;
			bool result = await ConnectUser ();
			_connected = result;
			return result;
		}

		/// <summary>
		/// Connect user to WebSite with user identifier (Login/Password)
		/// </summary>
		private async Task<bool> ConnectUser()
		{
			bool result = false;
			try
			{
				//Use credential http cookie
				var postData = new List<KeyValuePair<string, string>>();
				postData.Add(new KeyValuePair<string, string>("userName", _userName));
				postData.Add(new KeyValuePair<string, string>("password", _password));

				HttpContent content = new FormUrlEncodedContent(postData);
				var response = await _httpClient.PostAsync(_baseUrl + _relativeLoginUrl, content);

				if(response != null)
				{
					if(response.StatusCode == HttpStatusCode.Forbidden)
					{
						var messenger = Mvx.Resolve<IMvxMessenger> ();
						messenger.Publish (new MvxMessageLoginEntry (this));
					}
					else if(response.StatusCode == HttpStatusCode.OK)
						result = true;
				}
			}
			catch(Exception exception)
			{
				//TODO LOG
			}
			return result;
		}

		public async Task<T> GetAsync<T>(string relativeUrl)
		{
			T result = default(T);
			try
			{
				if(!_connected)
				{
					if(string.IsNullOrWhiteSpace(_userName) || string.IsNullOrWhiteSpace(_password))
					{
						var messenger = Mvx.Resolve<IMvxMessenger> ();
						messenger.Publish (new MvxMessageLoginEntry (this));
						throw new Exception("Connexion impossible");
					}
					else
						_connected = await ConnectUser();
				}
				
				if(!_connected)
					throw new Exception("Connexion impossible");
				
				var httpResponse = await _httpClient.GetAsync(_baseUrl + relativeUrl);

				if(httpResponse != null)
				{
					if(httpResponse.StatusCode == HttpStatusCode.OK)
					{
						var jsonStringResult = httpResponse.Content.ReadAsStringAsync().Result;
						result = JsonConvert.DeserializeObject<T>(jsonStringResult);
					}
					else if(httpResponse.StatusCode == HttpStatusCode.NotFound)
					{
						throw new Exception("Ressource not found");
					}
					else if(httpResponse.StatusCode == HttpStatusCode.Forbidden)
					{
						_connected = false;
						throw new Exception("Ressource forbidden");
					}
					else
					{
						throw new Exception("HTTP request error");
					}
				}
			}
			catch(Exception exception)
			{
				throw exception;
			}

			return result;
		}
	}
}


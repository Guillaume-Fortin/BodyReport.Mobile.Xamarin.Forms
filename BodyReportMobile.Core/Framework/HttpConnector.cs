using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;

namespace BodyReportMobile.Core
{
	public class HttpConnector
	{
		private string _baseUrl = "http://localhost:5000/";
		private const string _relativeLoginUrl = "Api/Account/Login";
		private HttpClient _httpClient = null;
		private bool _connected = false;

		public HttpConnector ()
		{
			//Now we make the same request with the token received by the auth service.
			CookieContainer cookies = new CookieContainer();
			HttpClientHandler handler = new HttpClientHandler();
			handler.CookieContainer = cookies;

			_httpClient = new System.Net.Http.HttpClient (handler);
		}

		/// <summary>
		/// Connect user to WebSite with user identifier (Login/Password)
		/// </summary>
		private async Task ConnectUser()
		{
			try
			{
				_connected = false;
				//Use credential http cookie
				var postData = new List<KeyValuePair<string, string>>();
				postData.Add(new KeyValuePair<string, string>("userName", "thetyne"));
				postData.Add(new KeyValuePair<string, string>("password", "azerty"));

				HttpContent content = new FormUrlEncodedContent(postData);
				var response = await _httpClient.PostAsync(_baseUrl + _relativeLoginUrl, content);

				if(response != null)
					_connected = response != null && response.StatusCode == HttpStatusCode.OK;
			}
			catch(Exception exception)
			{
				throw exception;
			}
		}

		public async Task<T> GetAsync<T>(string relativeUrl)
		{
			T result = default(T);
			try
			{
				if(!_connected)
					await ConnectUser();
				
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


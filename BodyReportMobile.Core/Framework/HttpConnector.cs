using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using XLabs.Ioc;
using BodyReportMobile.Core.MvxMessages;
using Message.WebApi;

namespace BodyReportMobile.Core.Framework
{
	public class HttpConnector
	{
        //private string _baseUrl = "http://192.168.0.15:5000/";
        private string _baseUrl = "http://192.168.1.11:5000/";
        private const string _relativeLoginUrl = "Api/Account/Login";
		private HttpClient _httpClient = null;
		private bool _connected = false;

		private string _userName = string.Empty;
		private string _password = string.Empty;

		private static HttpConnector _instance = null;

		public static HttpConnector Instance
		{
			get
			{
				if (_instance == null)
					_instance = new HttpConnector ();
				return _instance;
			}
		}

		private HttpConnector ()
		{
			//Now we make the same request with the token received by the auth service.
			CookieContainer cookies = new CookieContainer ();
			HttpClientHandler handler = new HttpClientHandler ();
			handler.CookieContainer = cookies;

			_httpClient = new System.Net.Http.HttpClient (handler);
            _httpClient.BaseAddress = new Uri(_baseUrl);
            _httpClient.Timeout = TimeSpan.FromSeconds(30);
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json")); 
		}

		public async Task<bool> ConnectUser (string userName, string password)
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
		private async Task<bool> ConnectUser ()
		{
			bool result = false;
			try
			{
				//Use credential http cookie
				var postData = new List<KeyValuePair<string, string>> ();
				postData.Add (new KeyValuePair<string, string> ("userName", _userName));
				postData.Add (new KeyValuePair<string, string> ("password", _password));

				HttpContent content = new FormUrlEncodedContent (postData);
				var response = await _httpClient.PostAsync (_relativeLoginUrl, content);

				if (response != null)
				{
					if (response.StatusCode == HttpStatusCode.Forbidden)
					{
                        AppMessenger.AppInstance.Send(new MvxMessageLoginEntry());
					}
					else if (response.StatusCode == HttpStatusCode.OK)
						result = true;
				}
			}
			catch (Exception exception)
			{
				//TODO LOG
			}
			return result;
		}

		private async Task AutoConnect()
		{
			if (!_connected)
			{
				if (string.IsNullOrWhiteSpace (_userName) || string.IsNullOrWhiteSpace (_password))
				{
                    AppMessenger.AppInstance.Send(new MvxMessageLoginEntry());
					throw new Exception ("Connexion impossible");
				}
				else
					_connected = await ConnectUser ();
			}
			if (!_connected)
				throw new Exception ("Connexion impossible");
		}

		public async Task<T> GetAsync<T> (string relativeUrl, Dictionary<string,string> datas = null)
		{
			T result = default(T);
			try
			{
				await AutoConnect();

                HttpResponseMessage httpResponse;
                if (datas != null && datas.Count > 0)
                {
                    var postData = new List<KeyValuePair<string, string>>();
                    postData.AddRange(datas);
                    HttpContent content = new FormUrlEncodedContent(postData);
                    string urlDatas = await content.ReadAsStringAsync();
                    httpResponse = await _httpClient.GetAsync(string.Format("{0}?{1}", relativeUrl, urlDatas));
                }
                else
                    httpResponse = await _httpClient.GetAsync(relativeUrl);

                if (httpResponse != null)
				{
					if (httpResponse.StatusCode == HttpStatusCode.OK)
					{
						var jsonStringResult = httpResponse.Content.ReadAsStringAsync ().Result;
                        if (!string.IsNullOrEmpty(jsonStringResult))
                        {
                            result = JsonConvert.DeserializeObject<T>(jsonStringResult);
                        }
					}
                    else if (httpResponse.StatusCode == HttpStatusCode.NoContent)
                    {
                        result = default(T);
                    }
                    else if (httpResponse.StatusCode == HttpStatusCode.NotFound)
					{
						throw new Exception ("Ressource not found");
					}
					else if (httpResponse.StatusCode == HttpStatusCode.Forbidden)
					{
						_connected = false;
						throw new Exception ("Ressource forbidden");
					}
                    else if (httpResponse.StatusCode == HttpStatusCode.BadRequest)
                    {
                        var jsonStringResult = httpResponse.Content.ReadAsStringAsync().Result;
                        if (!string.IsNullOrEmpty(jsonStringResult))
                        {
                            WebApiException webApiException = JsonConvert.DeserializeObject<WebApiException>(jsonStringResult);
                            if(webApiException != null)
                                throw webApiException;
                        }
                        throw new Exception("Bad HTTP request");
                    }
                    else
					{
						throw new Exception ("HTTP request error");
					}
				}
			}
            catch (TaskCanceledException timeoutException)
            {
                throw new Exception("HTTP timeout error");
            }
            catch (Exception exception)
			{
				throw exception;
			}

			return result;
		}

		public async Task<TResultData> PostAsync<TData, TResultData> (string relativeUrl, TData postData)
		{
            TResultData result = default(TResultData);
			try
			{
				await AutoConnect();

				string postBody = JsonConvert.SerializeObject(postData, new JsonSerializerSettings {Formatting = Formatting.None}); 
                var httpResponse = await _httpClient.PostAsync(relativeUrl, new StringContent(postBody, Encoding.UTF8, "application/json")); 

				if (httpResponse != null)
				{
					if (httpResponse.StatusCode == HttpStatusCode.OK)
					{
						var jsonStringResult = httpResponse.Content.ReadAsStringAsync ().Result;
                        if (!string.IsNullOrEmpty(jsonStringResult))
                        {
                            result = JsonConvert.DeserializeObject<TResultData>(jsonStringResult);
                        }
					}
                    else if (httpResponse.StatusCode == HttpStatusCode.NoContent)
                    {
                        result = default(TResultData);
                    }
                    else if (httpResponse.StatusCode == HttpStatusCode.NotFound)
					{
						throw new HttpException((int)httpResponse.StatusCode, "Ressource not found");
					}
					else if (httpResponse.StatusCode == HttpStatusCode.Forbidden)
					{
						_connected = false;
						throw new HttpException((int)httpResponse.StatusCode, "Ressource forbidden");
					}
                    else if (httpResponse.StatusCode == HttpStatusCode.BadRequest)
                    {
                        var jsonStringResult = httpResponse.Content.ReadAsStringAsync().Result;
                        if(!string.IsNullOrEmpty(jsonStringResult) && jsonStringResult.Contains("WebApiException"))
                        {
                            WebApiException webApiException = JsonConvert.DeserializeObject<WebApiException>(jsonStringResult);
                            if(webApiException != null)
                                throw webApiException;
                        }
                        throw new HttpException((int)httpResponse.StatusCode, "Bad request");
                    }
                    else
					{
						throw new HttpException((int)httpResponse.StatusCode, "error");
					}
				}
			}
            catch (TaskCanceledException timeoutException)
            {
                throw new HttpException("Timeout", timeoutException);
            }
            catch (Exception exception)
			{
				throw exception;
			}

			return result;
		}
    }
}


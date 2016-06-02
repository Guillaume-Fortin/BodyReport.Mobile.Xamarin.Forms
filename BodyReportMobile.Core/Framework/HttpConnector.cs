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
using BodyReport.Message.WebApi;
using System.IO;
using BodyReport.Message;

namespace BodyReportMobile.Core.Framework
{
    public class HttpConnector
    {
        private string _baseUrl = null;
        private const string _relativeLoginUrl = "Api/Account/Login";
        private HttpClient _httpClient = null;
        private bool _connected = false;

        private string _userName = string.Empty;
        private string _password = string.Empty;
        private LangType _langType = LangType.en_US;

        private static HttpConnector _instance = null;

        public static HttpConnector Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new HttpConnector();
                return _instance;
            }
        }

        /// <summary>
        /// Like http://www.bodyreport.org:5000/ but prefer use ip (domain very slow in mobile)
        /// </summary>
        public string BaseUrl
        {
            get
            {
                return _baseUrl;
            }

            set
            {
                _baseUrl = value;
                if(_httpClient != null)
                    _httpClient.BaseAddress = new Uri(_baseUrl);
            }
        }

        private HttpConnector()
        {
            //Now we make the same request with the token received by the auth service.
            CookieContainer cookies = new CookieContainer();
            HttpClientHandler handler = new HttpClientHandler();
            handler.CookieContainer = cookies;

            _httpClient = new HttpClient(handler);
            _httpClient.Timeout = TimeSpan.FromSeconds(30);
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.ExpectContinue = false;
        }

        public async Task<bool> ConnectUserAsync(string userName, string password, LangType langType, bool autoPromptLogin)
        {
            _userName = userName;
            _password = password;
            _langType = langType;
            bool result = await ConnectUserAsync(autoPromptLogin);
            _connected = result;
            return result;
        }

        /// <summary>
        /// Connect user to WebSite with user identifier (Login/Password)
        /// </summary>
        private async Task<bool> ConnectUserAsync(bool autoPromptLogin = true)
        {
            bool result = false;
            try
            {
                //Use credential http cookie
                var postData = new List<KeyValuePair<string, string>>();
                postData.Add(new KeyValuePair<string, string>("userName", _userName));
                postData.Add(new KeyValuePair<string, string>("password", _password));

                HttpContent content = new FormUrlEncodedContent(postData);
                _httpClient.DefaultRequestHeaders.Remove("Accept-Language");
                if(_langType == LangType.fr_FR)
                    _httpClient.DefaultRequestHeaders.Add("Accept-Language", "fr-FR");
                var response = await _httpClient.PostAsync(_relativeLoginUrl, content);

                if (response != null)
                {
                    if (response.StatusCode == HttpStatusCode.Forbidden)
                    {
                        if (autoPromptLogin)
                            AppMessenger.AppInstance.Send(new MvxMessageLoginEntry());
                    }
                    else if (response.StatusCode == HttpStatusCode.BadRequest)
                    {
                        var jsonStringResult = response.Content.ReadAsStringAsync().Result;
                        WebApiException webApiException = ConvertJsonExceptionToWebApiException(jsonStringResult);
                        if (webApiException != null)
                            throw webApiException;
                    }
                    else if (response.StatusCode == HttpStatusCode.OK)
                        result = true;
                }
            }
            catch (WebApiException webApiException)
            {
                throw webApiException;
            }
            catch (Exception except)
            {
                ILogger.Instance.Error("Unable to connect user", except);
            }
            return result;
        }

        private async Task AutoConnectAsync()
        {
            if (!_connected)
            {
                if (string.IsNullOrWhiteSpace(_userName) || string.IsNullOrWhiteSpace(_password))
                {
                    AppMessenger.AppInstance.Send(new MvxMessageLoginEntry());
                    throw new Exception("Can't connect to server");
                }
                else
                    _connected = await ConnectUserAsync();
            }
            if (!_connected)
                throw new Exception("Can't connect to server");
        }

        public async Task<T> GetAsync<T>(string relativeUrl, Dictionary<string, string> datas = null)
        {
            T result = default(T);
            try
            {
                await AutoConnectAsync();

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
                        var jsonStringResult = httpResponse.Content.ReadAsStringAsync().Result;
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
                        WebApiException webApiException = ConvertJsonExceptionToWebApiException(jsonStringResult);
                        if (webApiException != null)
                            throw webApiException;
                        throw new HttpException("Bad HTTP request");
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
                if (exception is HttpException || exception is WebApiException)
                    throw exception;
                throw new HttpException("Can't connect to server", exception);
            }

            return result;
        }

        public async Task<TResultData> PostAsync<TData, TResultData>(string relativeUrl, TData postData, bool isAnonymousRequest = false)
        {
            TResultData result = default(TResultData);
            try
            {
                if(!isAnonymousRequest)
                    await AutoConnectAsync();

                string postBody = JsonConvert.SerializeObject(postData, new JsonSerializerSettings { Formatting = Formatting.None });
                var httpResponse = await _httpClient.PostAsync(relativeUrl, new StringContent(postBody, Encoding.UTF8, "application/json"));

                if (httpResponse != null)
                {
                    if (httpResponse.StatusCode == HttpStatusCode.OK)
                    {
                        var jsonStringResult = httpResponse.Content.ReadAsStringAsync().Result;
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
                        WebApiException webApiException = ConvertJsonExceptionToWebApiException(jsonStringResult);
                        if (webApiException != null)
                            throw webApiException;
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
                if (exception is HttpException || exception is WebApiException)
                    throw exception;
                throw new HttpException("Can't connect to server", exception);
            }

            return result;
        }

        private WebApiException ConvertJsonExceptionToWebApiException(string jsonException)
        {
            WebApiException result = null;
            try
            {
                if (!string.IsNullOrEmpty(jsonException))
                {
                    result = JsonConvert.DeserializeObject<WebApiException>(jsonException);
                    if(result != null)
                    { // BUG with my exception, json parse bad field Message and InnerException
                        result = new WebApiException(result.Code, result.Message, result.InnerException);
                    }
                }
            }
            catch(Exception except)
            {
                ILogger.Instance.Error("Unable to convert http webapi error", except);
            }
            return result;
        }

        public async Task<string> UpLoadFileAsync(string relativeUrl, string filePath, string contentType)
        {
            try
            {
                var fileManager = Resolver.Resolve<IFileManager>();
                MultipartFormDataContent form = new MultipartFormDataContent();
                /*HttpContent content = new StringContent("fileToUpload");
                form.Add(content, "imageFile");*/

                using (var stream = fileManager.OpenFile(filePath))
                {
                    HttpContent content = new StreamContent(stream);
                    content.Headers.ContentType = new MediaTypeHeaderValue(contentType);
                    content.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                    {
                        Name = "imageFile",
                        FileName = Path.GetFileName(filePath),
                    };
                    form.Add(content);

                    var httpResponse = await _httpClient.PostAsync(relativeUrl, form);

                    if (httpResponse != null)
                    {
                        if (httpResponse.StatusCode == HttpStatusCode.OK)
                        {
                            string resultRelativeUrl = httpResponse.Content.ReadAsStringAsync().Result;
                            return resultRelativeUrl;
                        }
                        else
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
                if (exception is HttpException || exception is WebApiException)
                    throw exception;
                throw new HttpException("Can't connect to server", exception);
            }
            return null;
        }

        public async Task<bool> DownloadFileAsync(string relativeUrl, string filePath, bool anonymousCalling=false)
        {
            bool result = false;

            try
            {
                var fileManager = Resolver.Resolve<IFileManager>();

                HttpClient httpClient = _httpClient;
                if (anonymousCalling)
                {
                    httpClient = new HttpClient();
                    httpClient.BaseAddress = new Uri(BaseUrl);
                    httpClient.DefaultRequestHeaders.ExpectContinue = false;
                }

                using (Stream contentStream = await httpClient.GetStreamAsync(relativeUrl))
                {
                    if (fileManager.FileExist(filePath))
                        fileManager.DeleteFile(filePath);

                    using (Stream fileStream = fileManager.OpenFile(filePath))
                    {
                        await contentStream.CopyToAsync(fileStream);
                    }
                }
                return true;
            }
            catch (Exception except)
            {
                ILogger.Instance.Error("Unable to download file", except);
            }
            return result;
        }
    }
}


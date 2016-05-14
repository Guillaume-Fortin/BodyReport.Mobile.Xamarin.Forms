using System;
using System.Collections.Generic;
using Message;
using System.IO;
using Newtonsoft.Json;
using XLabs.Ioc;
using SQLite.Net;
using BodyReportMobile.Core.ServiceManagers;

namespace BodyReportMobile.Core.Framework
{
	public static class Translation
	{
		public delegate void ChangeHandler();
		public static event ChangeHandler ChangeEvent;

		private static Dictionary<string, Dictionary<string, string>> _translationDicoList = new Dictionary<string, Dictionary<string, string>>();
		private static Dictionary<string, string> _currentTranslation = null;

        private static LangType _currentLang;
        public static LangType CurrentLang { get { return _currentLang; } }

        /// <summary>
        /// Supported culture names
        /// </summary>
        public readonly static string[] SupportedCultureNames = new string[] { "en-US", "fr-FR" };
        
        private static SQLiteConnection _dbContext = null;

        static Translation ()
        {
            _dbContext = Resolver.Resolve<ISQLite>().GetConnection();
        }

		public static string GetLangExt(LangType langType)
		{
			return GetLangExt (langType.ToString ());
		}

		private static string GetLangExt(string langType)
		{
			return langType.Replace ("_", "-");
		}

		/// <summary>
		/// Load Translation inside Json file
		/// </summary>
		/// <returns></returns>
		public static bool LoadTranslation()
		{
			bool result = false;
			string filePath;
			string[] extensionList = Enum.GetNames (typeof(LangType));
			for (int i = 0; i < extensionList.Length; i++)
				extensionList [i] = GetLangExt (extensionList [i]);

			try
			{
				var fileManager = Resolver.Resolve<IFileManager>();
				foreach(string extension in extensionList)
				{
					//if(!_translationDicoList.ContainsKey(extension))
					//	_translationDicoList.Add(extension, new Dictionary<string, string>());

					filePath = string.Format("Translation-{0}.json", extension);
					filePath = Path.Combine(fileManager.GetResourcesPath(), filePath);
					if(fileManager.ResourceFileExist(filePath))
					{
                        using (StreamReader sr = new StreamReader(fileManager.OpenResourceFile(filePath)))
                        using (JsonTextReader reader = new JsonTextReader(sr))
                        {
                            JsonSerializer serializer = new JsonSerializer();
                            var translationDico = serializer.Deserialize<Dictionary<string, string>>(reader);
                            _translationDicoList.Add(extension, translationDico);
                            result = true;
                        }
					}
				}
			}
			catch//(Exception except)
			{
			}
            
			ChangeLang (CurrentLang);

			return result;
		}

		public static void ChangeLang(LangType langType)
		{
            _currentLang = langType;

            if(_translationDicoList.ContainsKey(GetLangExt(langType)))
			    _currentTranslation = _translationDicoList[GetLangExt(langType)];

			if (ChangeEvent != null)
				ChangeEvent ();
		}

        /// <summary>
        /// Get translation in json file
        /// </summary>
        /// <param name="key">Translation key</param>
        /// <returns>Transaltion value</returns>
        public static string Get(string key, bool returnNullIfNotExist = false)
		{
			if (_currentTranslation != null && _currentTranslation.ContainsKey(key))
				return _currentTranslation [key];
			else
				return returnNullIfNotExist ? null : '*' +key;
		}

        private static int GetCurrentCultureId()
        {
            int currentCultureId = 0;
            string culture = _currentLang.ToString().Replace("_", "-");
            for (int i = 0; i < SupportedCultureNames.Length; i++)
            {
                if (SupportedCultureNames[i].ToLower() == culture.ToLower())
                {
                    currentCultureId = i;
                    break;
                }
            }
            return currentCultureId;
        }

        /// <summary>
        /// Get translation in database
        /// </summary>
        /// <param name="key">Translation key</param>
        /// <returns>Transaltion value</returns>
        public static string GetInDB(string key)
        {
            string result = Get("DB_" + key, true);
            if (result == null)
            {
                try
                {
                    int currentCultureId = GetCurrentCultureId();

                    var translationManager = new TranslationManager(_dbContext);
                    var translationValKey = new TranslationValKey()
                    {
                        CultureId = currentCultureId,
                        Key = key
                    };
                    var trValue = translationManager.GetTranslation(translationValKey);

                    if (trValue != null & trValue.Value != null)
                        result = trValue.Value;
                    else
                        ILogger.Instance.Info(string.Format("Translation database not found {0}", key));
                }
                catch (Exception except)
                {
                    ILogger.Instance.Error("Get translation database error", except);
                }
                //Add translation in memory
                if (result != null)
                    _currentTranslation.Add("DB_" + key, result);
            }

            return result;
        }
    }
}


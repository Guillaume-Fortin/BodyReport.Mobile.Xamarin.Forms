using System;
using System.Collections.Generic;
using Message;
using System.IO;
using Newtonsoft.Json;
using XLabs.Ioc;

namespace BodyReportMobile.Core.Framework
{
	public static class Translation
	{
		public delegate void ChangeHandler();
		public static event ChangeHandler ChangeEvent;

		private static Dictionary<string, Dictionary<string, string>> _translationDicoList = new Dictionary<string, Dictionary<string, string>>();
		private static Dictionary<string, string> _currentTranslation = null;
		public static LangType CurrentLang { get; set; }

		static Translation ()
		{

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
					if(fileManager.FileExist(filePath))
					{
						using (StreamReader sr = fileManager.OpenFile(filePath))
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

			CurrentLang = LangType.en_US;
			ChangeLang (CurrentLang);

			return result;
		}

		public static void ChangeLang(LangType langType)
		{
			CurrentLang = langType;

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
		public static string Get(string key)
		{
			if (_currentTranslation != null && _currentTranslation.ContainsKey(key))
				return _currentTranslation [key];
			else
				return '*'+key;
		}
	}
}


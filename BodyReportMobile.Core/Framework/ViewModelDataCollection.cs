using System;
using System.Collections.Generic;

namespace BodyReportMobile.Core.Framework
{
	public static class ViewModelDataCollection
	{
		private static Dictionary<string, Dictionary<string, object>> _dataDictionary = new Dictionary<string, Dictionary<string, object>>();

		public static void Push<T>(string viewModelGuid, string name, T value)
		{
			if (!_dataDictionary.ContainsKey (viewModelGuid))
				_dataDictionary.Add (viewModelGuid, new Dictionary<string, object> ());

			var objectDictionary = _dataDictionary[viewModelGuid];
			objectDictionary.Add (name, value);
		}

		public static T Get<T>(string viewModelGuid, string name)
		{
			T value = default(T);

			if (_dataDictionary.ContainsKey (viewModelGuid)) {
				
				var objectDictionary = _dataDictionary [viewModelGuid];
				if (objectDictionary.ContainsKey (name)) {
					value = (T)objectDictionary [name];
				}
			}

			return value;
		}

		public static void Clear(string viewModelGuid)
		{
			if (viewModelGuid != null && _dataDictionary.ContainsKey (viewModelGuid)) {
				_dataDictionary.Remove (viewModelGuid);
			}
		}
	}
}


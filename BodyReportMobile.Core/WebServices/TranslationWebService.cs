using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Message;

namespace BodyReportMobile.Core
{
	public static class TranslationWebService
	{
		public static async Task<List<TranslationVal>> FindTranslations ()
		{
			var httpConnector = new HttpConnector ();

			return await httpConnector.GetAsync<List<TranslationVal>> ("api/Translations/Find");
		}
	}
}


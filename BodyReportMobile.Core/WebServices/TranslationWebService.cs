using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using BodyReport.Message;
using BodyReportMobile.Core.Framework;

namespace BodyReportMobile.Core.WebServices
{
	public static class TranslationWebService
	{
		public static async Task<List<TranslationVal>> FindTranslationsAsync ()
		{
			return await HttpConnector.Instance.GetAsync<List<TranslationVal>> ("api/Translations/Find");
		}
	}
}


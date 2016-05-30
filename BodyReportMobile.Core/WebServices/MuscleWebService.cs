using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using BodyReport.Message;
using BodyReportMobile.Core.Framework;

namespace BodyReportMobile.Core.WebServices
{
	public static class MuscleWebService
	{
		public static async Task<List<Muscle>> FindMusclesAsync()
		{
			return await HttpConnector.Instance.GetAsync<List<Muscle>> ("api/Muscles/Find");
		}

	}
}


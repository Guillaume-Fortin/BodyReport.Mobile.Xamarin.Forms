using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Message;
using BodyReportMobile.Core.Framework;

namespace BodyReportMobile.Core.WebServices
{
	public static class MuscleWebService
	{
		public static async Task<List<Muscle>> FindMuscles ()
		{
			return await HttpConnector.Instance.GetAsync<List<Muscle>> ("api/Muscles/Find");
		}

	}
}


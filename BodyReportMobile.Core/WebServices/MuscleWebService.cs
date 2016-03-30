using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Message;

namespace BodyReportMobile.Core
{
	public static class MuscleWebService
	{
		public static async Task<List<Muscle>> FindMuscles ()
		{
			return await HttpConnector.Instance.GetAsync<List<Muscle>> ("api/Muscles/Find");
		}

	}
}


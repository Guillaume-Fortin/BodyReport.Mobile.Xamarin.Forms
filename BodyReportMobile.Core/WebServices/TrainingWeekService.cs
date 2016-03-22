using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Message;

namespace BodyReportMobile.Core
{
	public static class TrainingWeekService
	{
		public static async Task<List<TrainingWeek>> FindTrainingWeeks ()
		{
			var httpConnector = new HttpConnector ();

			return await httpConnector.GetAsync<List<TrainingWeek>> ("api/TrainingWeeks/Find");
		}
	}
}


using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Message;
using BodyReportMobile.Core.Framework;

namespace BodyReportMobile.Core.WebServices
{
	public static class TrainingWeekService
	{
		public static async Task<List<TrainingWeek>> FindTrainingWeeks ()
		{
			return await HttpConnector.Instance.GetAsync<List<TrainingWeek>> ("Api/TrainingWeeks/Find");
		}

        public static async Task<TrainingWeek> CreateTrainingWeek(TrainingWeek trainingWeek)
        {
            return await HttpConnector.Instance.PostAsync<TrainingWeek, TrainingWeek>("Api/TrainingWeeks/Create", trainingWeek);
        }

        public static async Task<TrainingWeek> UpdateTrainingWeek (TrainingWeek trainingWeek)
		{
			return await HttpConnector.Instance.PostAsync<TrainingWeek, TrainingWeek> ("Api/TrainingWeeks/Update", trainingWeek);
		}

        public static async Task DeleteTrainingWeekByKey(TrainingWeekKey trainingWeekKey)
        {
            await HttpConnector.Instance.PostAsync<TrainingWeekKey, object>("Api/TrainingWeeks/DeleteByKey", trainingWeekKey);
            return;
        }
    }
}


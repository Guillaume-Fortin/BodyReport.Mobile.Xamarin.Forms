using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Message;
using BodyReportMobile.Core.Framework;
using Message.WebApi;

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

        public static async Task<TrainingWeek> GetTrainingWeek(TrainingWeekKey key)
        {
            if (key == null)
                return null;

            Dictionary<string, string> datas = new Dictionary<string, string>();
            datas.Add("UserId", key.UserId);
            datas.Add("Year", key.Year.ToString());
            datas.Add("WeekOfYear", key.WeekOfYear.ToString());
            return await HttpConnector.Instance.GetAsync<TrainingWeek>("Api/TrainingWeeks/Get", datas);
        }

        internal static async Task<TrainingWeek> CopyTrainingWeek(CopyTrainingWeek copyTrainingWeek)
        {
            if (copyTrainingWeek == null)
                return null;

            return await HttpConnector.Instance.PostAsync<CopyTrainingWeek, TrainingWeek>("Api/TrainingWeeks/Copy", copyTrainingWeek);
        }
    }
}


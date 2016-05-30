using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using BodyReport.Message;
using BodyReportMobile.Core.Framework;
using BodyReport.Message.WebApi;
using BodyReport.Message.WebApi.MultipleParameters;

namespace BodyReportMobile.Core.WebServices
{
	public static class TrainingWeekWebService
	{
		public static async Task<List<TrainingWeek>> FindTrainingWeeksAsync (CriteriaList<TrainingWeekCriteria> trainingWeekCriteriaList, TrainingWeekScenario trainingWeekScenario)
		{
            if (trainingWeekCriteriaList == null)
                return null;
            
            var trainingWeekFinder = new TrainingWeekFinder();
            trainingWeekFinder.TrainingWeekCriteriaList = trainingWeekCriteriaList;
            trainingWeekFinder.TrainingWeekScenario = trainingWeekScenario;
            return await HttpConnector.Instance.PostAsync<TrainingWeekFinder, List<TrainingWeek>> ("Api/TrainingWeeks/Find", trainingWeekFinder);
		}

        public static async Task<TrainingWeek> CreateTrainingWeekAsync(TrainingWeek trainingWeek)
        {
            return await HttpConnector.Instance.PostAsync<TrainingWeek, TrainingWeek>("Api/TrainingWeeks/Create", trainingWeek);
        }

        public static async Task<TrainingWeek> UpdateTrainingWeekAsync (TrainingWeek trainingWeek)
		{
			return await HttpConnector.Instance.PostAsync<TrainingWeek, TrainingWeek> ("Api/TrainingWeeks/Update", trainingWeek);
		}

        public static async Task DeleteTrainingWeekByKeyAsync(TrainingWeekKey trainingWeekKey)
        {
            await HttpConnector.Instance.PostAsync<TrainingWeekKey, object>("Api/TrainingWeeks/DeleteByKey", trainingWeekKey);
            return;
        }

        public static async Task<TrainingWeek> GetTrainingWeekAsync(TrainingWeekKey key, bool manageDay=false)
        {
            if (key == null)
                return null;

            Dictionary<string, string> datas = new Dictionary<string, string>();
            datas.Add("UserId", key.UserId);
            datas.Add("Year", key.Year.ToString());
            datas.Add("WeekOfYear", key.WeekOfYear.ToString());
            datas.Add("manageDay", manageDay.ToString());
            return await HttpConnector.Instance.GetAsync<TrainingWeek>("Api/TrainingWeeks/Get", datas);
        }

        internal static async Task<TrainingWeek> CopyTrainingWeekAsync(CopyTrainingWeek copyTrainingWeek)
        {
            if (copyTrainingWeek == null)
                return null;

            return await HttpConnector.Instance.PostAsync<CopyTrainingWeek, TrainingWeek>("Api/TrainingWeeks/Copy", copyTrainingWeek);
        }
    }
}


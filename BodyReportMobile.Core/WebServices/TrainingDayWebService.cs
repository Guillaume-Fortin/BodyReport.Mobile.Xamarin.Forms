using BodyReportMobile.Core.Framework;
using Message;
using Message.WebApi.MultipleParameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BodyReportMobile.Core.WebServices
{
    public class TrainingDayWebService
    {
        public static async Task<List<TrainingDay>> FindTrainingDaysAsync(TrainingDayCriteria trainingDayCriteria, TrainingDayScenario trainingDayScenario)
        {
            if (trainingDayCriteria == null)
                return null;

            var trainingDayFinder = new TrainingDayFinder();
            trainingDayFinder.TrainingDayCriteria = trainingDayCriteria;
            trainingDayFinder.TrainingDayScenario = trainingDayScenario;
            return await HttpConnector.Instance.PostAsync<TrainingDayFinder, List<TrainingDay>>("Api/TrainingDays/Find", trainingDayFinder);
        }

        public static async Task<TrainingDay> CreateTrainingDaysAsync(TrainingDay trainingDay)
        {
            return await HttpConnector.Instance.PostAsync<TrainingDay, TrainingDay>("Api/TrainingDays/Create", trainingDay);
        }

        public static async Task<TrainingDay> GetTrainingDayAsync(TrainingDayKey key, bool manageExercise = false)
        {
            if (key == null)
                return null;

            Dictionary<string, string> datas = new Dictionary<string, string>();
            datas.Add("UserId", key.UserId);
            datas.Add("Year", key.Year.ToString());
            datas.Add("WeekOfYear", key.WeekOfYear.ToString());
            datas.Add("DayOfWeek", key.DayOfWeek.ToString());
            datas.Add("TrainingDayId", key.TrainingDayId.ToString());
            datas.Add("manageExercise", manageExercise.ToString());
            return await HttpConnector.Instance.GetAsync<TrainingDay>("Api/TrainingDays/Get", datas);
        }
    }
}

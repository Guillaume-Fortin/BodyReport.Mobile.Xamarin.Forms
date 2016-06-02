using BodyReportMobile.Core.Framework;
using BodyReport.Message;
using BodyReport.Message.WebApi.MultipleParameters;
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

        public static async Task<TrainingDay> GetTrainingDayAsync(TrainingDayKey key, TrainingDayScenario trainingDayScenario)
        {
            if (key == null || trainingDayScenario == null)
                return null;

            Dictionary<string, string> datas = new Dictionary<string, string>();
            datas.Add("UserId", key.UserId);
            datas.Add("Year", key.Year.ToString());
            datas.Add("WeekOfYear", key.WeekOfYear.ToString());
            datas.Add("DayOfWeek", key.DayOfWeek.ToString());
            datas.Add("TrainingDayId", key.TrainingDayId.ToString());
            datas.Add("ManageExercise", trainingDayScenario.ManageExercise.ToString());
            return await HttpConnector.Instance.GetAsync<TrainingDay>("Api/TrainingDays/Get", datas);
        }

        internal static async Task<TrainingDay> UpdateTrainingDayAsync(TrainingDay trainingDay, TrainingDayScenario trainingDayScenario)
        {
            if (trainingDay == null)
                return null;

            var trainingDayWithScenario = new TrainingDayWithScenario()
            {
                TrainingDay = trainingDay,
                TrainingDayScenario = trainingDayScenario
            };
            return await HttpConnector.Instance.PostAsync<TrainingDayWithScenario, TrainingDay>("Api/TrainingDays/Update", trainingDayWithScenario);
        }

        public static async Task<bool> DeleteTrainingDayAsync(TrainingDayKey trainingDayKey)
        {
            return await HttpConnector.Instance.PostAsync<TrainingDayKey, bool>("Api/TrainingDays/Delete", trainingDayKey);
        }

        internal static async Task<bool> SwitchDayOfTrainingDay(TrainingDayKey trainingDayKey, int switchDayOfWeek)
        {
            if (trainingDayKey == null)
                return false;

            var switchDayParameter = new SwitchDayParameter()
            {
                TrainingDayKey = trainingDayKey,
                SwitchDayOfWeek = switchDayOfWeek
            };

            return await HttpConnector.Instance.PostAsync<SwitchDayParameter, bool>("Api/TrainingDays/SwitchDay", switchDayParameter);
        }
    }
}

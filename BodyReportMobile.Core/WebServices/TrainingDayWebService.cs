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
        public static async Task<List<TrainingDay>> FindTrainingDays(TrainingDayCriteria trainingDayCriteria, TrainingDayScenario trainingDayScenario)
        {
            if (trainingDayCriteria == null)
                return null;

            var trainingDayFinder = new TrainingDayFinder();
            trainingDayFinder.TrainingDayCriteria = trainingDayCriteria;
            trainingDayFinder.TrainingDayScenario = trainingDayScenario;
            return await HttpConnector.Instance.PostAsync<TrainingDayFinder, List<TrainingDay>>("Api/TrainingDays/Find", trainingDayFinder);
        }

        public static async Task<TrainingDay> CreateTrainingDays(TrainingDay trainingDay)
        {
            return await HttpConnector.Instance.PostAsync<TrainingDay, TrainingDay>("Api/TrainingDays/Create", trainingDay);
        }
    }
}

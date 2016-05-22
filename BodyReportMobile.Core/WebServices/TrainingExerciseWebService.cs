using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Message;
using BodyReportMobile.Core.Framework;

namespace BodyReportMobile.Core.WebServices
{
    public static class TrainingExerciseWebService
    {
        public static async Task DeleteTrainingExerciseAsync(TrainingExerciseKey trainingExerciseKey)
        {
            await HttpConnector.Instance.PostAsync<TrainingExerciseKey, object>("Api/TrainingExercises/DeleteByKey", trainingExerciseKey);
        }
    }
}

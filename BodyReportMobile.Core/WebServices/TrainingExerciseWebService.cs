using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BodyReport.Message;
using BodyReportMobile.Core.Framework;

namespace BodyReportMobile.Core.WebServices
{
    public static class TrainingExerciseWebService
    {
        public static async Task<bool> DeleteTrainingExerciseAsync(TrainingExerciseKey trainingExerciseKey)
        {
            return await HttpConnector.Instance.PostAsync<TrainingExerciseKey, bool>("Api/TrainingExercises/Delete", trainingExerciseKey);
        }
    }
}

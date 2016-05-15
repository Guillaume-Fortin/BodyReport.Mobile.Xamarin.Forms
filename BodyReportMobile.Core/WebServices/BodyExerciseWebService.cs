using BodyReportMobile.Core.Framework;
using Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BodyReportMobile.Core.WebServices
{
    public static class BodyExerciseWebService
    {
        public static async Task<List<BodyExercise>> FindBodyExercisesAsync()
        {
            return await HttpConnector.Instance.GetAsync<List<BodyExercise>>("api/BodyExercises/Find");
        }
    }
}

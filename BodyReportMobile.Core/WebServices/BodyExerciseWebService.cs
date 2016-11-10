using BodyReportMobile.Core.Framework;
using BodyReport.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BodyReportMobile.Core.WebServices
{
    public static class BodyExerciseWebService
    {
        public static async Task<List<BodyExercise>> FindAsync()
        {
            return await HttpConnector.Instance.GetAsync<List<BodyExercise>>("Api/BodyExercises/Find");
        }
    }
}

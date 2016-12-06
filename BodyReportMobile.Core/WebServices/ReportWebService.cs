using BodyReport.Message;
using BodyReport.Message.Web;
using BodyReportMobile.Core.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BodyReportMobile.Core.WebServices
{
    public class ReportWebService
    {
        /// <summary>
        /// Crée un report pdf
        /// </summary>
        /// <param name="trainingDayReport">report demandé</param>
        /// <returns>Url du rapport</returns>
        public static async Task<MemoryStream> TrainingDayReportAsync(TrainingDayReport trainingDayReport)
        {
            return await HttpConnector.Instance.PostAsync<TrainingDayReport, MemoryStream>("Api/Report/TrainingDayReport", trainingDayReport);
        }
    }
}

using BodyReportMobile.Core.Framework;
using Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BodyReportMobile.Core.WebServices
{
    public static class MuscularGroupWebService
    {
        internal static async Task<List<MuscularGroup>> FindMuscularGroups()
        {
            return await HttpConnector.Instance.GetAsync<List<MuscularGroup>>("Api/MuscularGroups/Find");
        }
    }
}

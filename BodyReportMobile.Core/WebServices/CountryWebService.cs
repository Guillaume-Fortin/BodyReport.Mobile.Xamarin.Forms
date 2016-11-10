using BodyReportMobile.Core.Framework;
using BodyReport.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BodyReportMobile.Core.WebServices
{
    public static class CountryWebService
    {
        public static async Task<List<Country>> FindCountriesAsync()
        {
            return await HttpConnector.Instance.GetAsync<List<Country>>("Api/Countries/Find");
        }
    }
}

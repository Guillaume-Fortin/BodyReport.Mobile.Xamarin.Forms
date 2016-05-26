using BodyReportMobile.Core.Models;
using Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BodyReportMobile.Core.Crud.Transformer
{
    public static class CountryTransformer
    {
        public static void ToRow(Country bean, CountryRow row)
        {
            if (bean == null)
                return;

            row.Id = bean.Id;
            row.ShortName = bean.ShortName;
            row.Name = bean.Name;
        }

        public static Country ToBean(CountryRow row)
        {
            if (row == null)
                return null;

            var bean = new Country();
            bean.Id = row.Id;
            bean.ShortName = row.ShortName;
            bean.Name = row.Name;
            return bean;
        }
    }
}

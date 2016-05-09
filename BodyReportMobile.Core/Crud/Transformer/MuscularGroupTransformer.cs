using BodyReportMobile.Core.Models;
using Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BodyReportMobile.Core.Crud.Transformer
{
    public class MuscularGroupTransformer
    {
        public static string GetTranslationKey(int muscularGroupId)
        {
            return string.Format("MG-{0}", muscularGroupId);
        }

        public static void ToRow(MuscularGroup bean, MuscularGroupRow row)
        {
            if (bean == null)
                return;

            row.Id = bean.Id;
        }

        internal static MuscularGroup ToBean(MuscularGroupRow row)
        {
            if (row == null)
                return null;

            var bean = new MuscularGroup();
            bean.Id = row.Id;
            return bean;
        }
    }
}

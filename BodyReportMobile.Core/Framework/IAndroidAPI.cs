using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BodyReportMobile.Core.Framework
{
    public interface IAndroidAPI
    {
        void CloseApp();
        void OpenPdf(string filePath);
    }
}

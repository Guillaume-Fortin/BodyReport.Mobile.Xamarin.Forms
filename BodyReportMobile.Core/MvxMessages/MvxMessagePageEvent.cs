using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BodyReportMobile.Core.MvxMessages
{
    public class MvxMessagePageEvent
    {
        public MvxMessagePageEvent(string viewModelGuid)
        {
            ViewModelGuid = viewModelGuid;
        }

        public string ViewModelGuid { get; private set; }
        public bool ClosingRequest { get; set; } = false;
        public bool ClosingRequest_ViewCanceled { get; set; } = false;
    }
}

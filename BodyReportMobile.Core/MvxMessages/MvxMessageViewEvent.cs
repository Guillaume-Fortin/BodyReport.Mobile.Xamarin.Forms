using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BodyReportMobile.Core.MvxMessages
{
    public class MvxMessageViewEvent
    {
        public MvxMessageViewEvent(string viewModelGuid)
        {
            ViewModelGuid = viewModelGuid;
        }

        public string ViewModelGuid { get; private set; }
        public bool Show { get; set; }
        public bool Appear { get; set; }
        public bool Disappear { get; set; }
        public bool Closing { get; set; }
        public bool Closed { get; set; }
    }
}

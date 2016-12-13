using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLabs.Ioc;

namespace BodyReportMobile.Core.Framework
{
    public abstract class IMedia
    {
        private static IMedia _instance = null;

        public static IMedia Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = Resolver.Resolve<IMedia>();
                }
                return _instance;
            }
        }

        public abstract void CompressImageAsPng(string imagePath, string compressedImagePath, int? maxSize);
		public abstract bool PrintDocumentFromWebView (object webView);
    }
}

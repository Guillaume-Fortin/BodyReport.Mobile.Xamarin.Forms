using Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLabs.Ioc;

namespace BodyReportMobile.Core.Framework
{
    public abstract class ILogger
    {
        private static ILogger _instance = null;

        public static ILogger Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = Resolver.Resolve<ILogger>();
                }
                return _instance;
            }
        }

        protected abstract void LogInFile(string text);

        public void Debug(string text, Exception exception = null)
        {
#if DEBUG
            Trace(TTraceLevel.Debug, text, exception);
#endif
        }


        public void Info(string text, Exception exception = null)
        {
            Trace(TTraceLevel.Info, text, exception);
        }

        public void Warning(string text, Exception exception = null)
        {
            Trace(TTraceLevel.Warning, text, exception);
        }

        public void Error(string text, Exception exception = null)
        {
            Trace(TTraceLevel.Error, text, exception);
        }


        public void Trace(TTraceLevel traceLevel, string text, Exception exception = null)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(string.Format("{0}/{1} : ", traceLevel.ToString(), DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss")));

            if (!string.IsNullOrWhiteSpace(text))
            {
                sb.Append(text);
                sb.AppendLine();
            }

            if (exception != null)
            {
                sb.Append(exception.ToString());
                sb.AppendLine();
            }

            if (traceLevel == TTraceLevel.Debug)
                System.Diagnostics.Debug.WriteLine(sb.ToString());
            LogInFile(sb.ToString());
        }
    }
}

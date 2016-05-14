using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using BodyReportMobile.Core.Framework;
using Android.Util;
using System.IO;

namespace BodyReport.Droid.Framework
{
    public class Logger : ILogger
    {
        private object _locker = new object();
        private string _fileName = "BodyReport.log";
        private bool _firstWrite = true;

        public Logger()
        {
            
        }

        protected override void LogInFile(string message)
        {
            lock (_locker)
            {
                try
                {
                    if (Android.OS.Environment.MediaMounted != Android.OS.Environment.ExternalStorageState)
                    {
                        Log.Debug("BodyReport", "Sdcard was not mounted !!");
                    }
                    else
                    {
                        string filePath = Path.Combine(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath, _fileName);
                        if (_firstWrite && File.Exists(filePath))
                        {
                            _firstWrite = false;
                            File.Delete(filePath);
                        }
                            
                        using (var streamWriter = new StreamWriter(filePath, true, Encoding.UTF8))
                        {
                            streamWriter.Write(message);
                        }
                    }
                }
                catch (Exception except)
                {
                    Log.Error("BodyReport", except.ToString());
                }
            }
        }
    }
}
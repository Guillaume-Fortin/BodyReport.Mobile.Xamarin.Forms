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
using System.IO;
using Xamarin.Forms;

namespace BodyReport.Droid
{
    public class AndroidAPI : IAndroidAPI
    {
        public void CloseApp()
        {
            Android.OS.Process.KillProcess(Android.OS.Process.MyPid());
        }

        public void OpenPdf(string filePath)
        {
            try
            {
                if (filePath == null || !File.Exists(filePath))
                    return;

                string newPath = Path.Combine(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath, Path.GetFileName(filePath));
                File.Copy(filePath, newPath);

                Java.IO.File file = new Java.IO.File(newPath);
                Android.Net.Uri path = Android.Net.Uri.FromFile(file);
                string extension = Android.Webkit.MimeTypeMap.GetFileExtensionFromUrl(Android.Net.Uri.FromFile(file).ToString());
                string mimeType = Android.Webkit.MimeTypeMap.Singleton.GetMimeTypeFromExtension(extension);
                Intent intent = new Intent(Intent.ActionView);
                intent.SetDataAndType(path, mimeType);
                Forms.Context.StartActivity(Intent.CreateChooser(intent, "Choose App"));
            }
            catch
            { }
        }
    }
}
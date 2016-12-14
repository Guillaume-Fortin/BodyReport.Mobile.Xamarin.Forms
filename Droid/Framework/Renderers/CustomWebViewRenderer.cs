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
using Xamarin.Forms.Platform.Android;
using BodyReport.Droid.Framework.Renderers;
using BodyReportMobile.Presenter.Framework.Controls;
using Xamarin.Forms;
using System.Net;
using System.IO;

[assembly: ExportRenderer(typeof(CustomWebView), typeof(CustomWebViewRenderer))]
namespace BodyReport.Droid.Framework.Renderers
{
    public class CustomWebViewRenderer : WebViewRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<WebView> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement != null)
            {
                var customWebView = Element as CustomWebView;
                customWebView.PlatformControl = Control;
                Control.Settings.AllowUniversalAccessFromFileURLs = true;

                /*string newPath = Path.Combine(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath, Path.GetFileName(customWebView.Uri));
                File.Copy(customWebView.Uri, newPath, true);
                Java.IO.File file = new Java.IO.File(newPath);
                Android.Net.Uri path = Android.Net.Uri.FromFile(file);*/

                Control.LoadUrl(string.Format("file:///android_asset/pdfjs/web/viewer.html?file={0}", WebUtility.UrlEncode(customWebView.Uri)));
            }
        }
    }
}
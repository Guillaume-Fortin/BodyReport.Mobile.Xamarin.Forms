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
using Xamarin.Forms;
using BodyReportMobile.Presenter.Pages;
using Android.Graphics.Drawables;

[assembly: ExportRenderer(typeof(BodyReportMobile.Presenter.Pages.BaseContentPage), typeof(BodyReport.Droid.Framework.Renderers.BasePageRenderer))]
namespace BodyReport.Droid.Framework.Renderers
{
    public class BasePageRenderer : PageRenderer
    {
        public BasePageRenderer()
        {
        }


        protected override void OnElementChanged(ElementChangedEventArgs<Page> e)
        {
           base.OnElementChanged(e);
            
            var activity = this.Context as Activity;

            var page = Element as BaseContentPage;

            LayoutInflater mInflater = LayoutInflater.From(activity);
        }
    }
}
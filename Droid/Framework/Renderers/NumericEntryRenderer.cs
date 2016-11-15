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
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using System.ComponentModel;

[assembly: ExportRenderer(typeof(BodyReportMobile.Presenter.Framework.Controls.NumericEntry), typeof(BodyReport.Droid.Framework.Renderers.NumericEntryRenderer))]
namespace BodyReport.Droid.Framework.Renderers
{
    public class NumericEntryRenderer : EntryRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                Control.SetSelectAllOnFocus(true);
                Control.InputType = Control.InputType | Android.Text.InputTypes.TextFlagNoSuggestions;
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (Control != null)
            {
                if (e.PropertyName == "Text" && !string.IsNullOrEmpty(Control.Text))
                {
                    Control.SetSelection(Control.Text.Length);
                }
            }
        }
    }
}
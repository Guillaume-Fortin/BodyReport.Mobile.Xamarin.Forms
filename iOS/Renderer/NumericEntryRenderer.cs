using System;
using Xamarin.Forms;
using BodyReport.iOS;
using Foundation;
using UIKit;
using Xamarin.Forms.Platform.iOS;
using BodyReportMobile.Presenter.Framework.Controls;

[assembly:ExportRenderer (typeof(NumericEntry), typeof(NumericEntryRenderer))]
namespace BodyReport.iOS
{
	public class NumericEntryRenderer : EntryRenderer
	{
		protected override void OnElementChanged (ElementChangedEventArgs<Entry> e)
		{
			base.OnElementChanged (e);

			if (Control != null) {
				Control.BorderStyle = UITextBorderStyle.None;
			}
		}
	}
}


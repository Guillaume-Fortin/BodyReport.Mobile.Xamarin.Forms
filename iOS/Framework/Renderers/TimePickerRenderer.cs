using System;
using Xamarin.Forms;
using BodyReportMobile.Presenter.Framework.Controls;
using BodyReport.iOS;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(TimePicker), typeof(BodyReport.iOS.Framework.Renderers.TimePickerRenderer))]
namespace BodyReport.iOS.Framework.Renderers
{
	public class TimePickerRenderer : Xamarin.Forms.Platform.iOS.TimePickerRenderer
	{
		protected override void OnElementChanged(ElementChangedEventArgs<TimePicker> e)
		{
			base.OnElementChanged (e);

			if (this.Control != null)
			{
				this.Control.BorderStyle = UIKit.UITextBorderStyle.None;
			}
		}
	}
}	


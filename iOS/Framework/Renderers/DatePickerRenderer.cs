using System;
using Xamarin.Forms.Platform.iOS;
using Xamarin.Forms;

[assembly: ExportRenderer(typeof(DatePicker), typeof(BodyReport.iOS.Framework.Renderers.DatePickerRenderer))]
namespace BodyReport.iOS.Framework.Renderers
{
	public class DatePickerRenderer : Xamarin.Forms.Platform.iOS.DatePickerRenderer
	{
		protected override void OnElementChanged(ElementChangedEventArgs<DatePicker> e)
		{
			base.OnElementChanged (e);

			if (this.Control != null)
			{
				this.Control.BorderStyle = UIKit.UITextBorderStyle.None;
			}
		}
	}
}


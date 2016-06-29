using System;
using Xamarin.Forms.Platform.iOS;
using Xamarin.Forms;
using UIKit;
using Foundation;
using BodyReportMobile.Presenter;

[assembly: ExportRenderer(typeof(DatePicker), typeof(BodyReport.iOS.Framework.Renderers.DatePickerRenderer))]
namespace BodyReport.iOS.Framework.Renderers
{
	public class DatePickerRenderer : Xamarin.Forms.Platform.iOS.DatePickerRenderer
	{
		UIColor _borderColor = UIColor.Gray;
		bool _disposed = false;
		bool _observerPresent = false;
		IntPtr tokenObserveBound = (IntPtr)1;

		public DatePickerRenderer()
		{
			Color color;
			if (App.GetXamlResources("entryBorderColor", out color))
				_borderColor = color.ToUIColor ();
		}

		protected override void Dispose (bool disposing)
		{
			_disposed = true;

			if (_observerPresent && Control != null && Control.Layer != null) {
				Control.Layer.RemoveObserver (this, "bounds", tokenObserveBound);
				_observerPresent = false;
			}
			base.Dispose (disposing);
		}

		protected override void OnElementChanged (ElementChangedEventArgs<DatePicker> e)
		{
			base.OnElementChanged (e);

			if (Control != null) {
				this.Control.BorderStyle = UITextBorderStyle.None;

				if (!_observerPresent && !_disposed && Control.Layer != null) {
					Control.Layer.AddObserver (this, (NSString)"bounds", NSKeyValueObservingOptions.New, tokenObserveBound);
					_observerPresent = true;
				}
			}
		}

		public override void ObserveValue (NSString keyPath, NSObject ofObject,
				   NSDictionary change, IntPtr context)
		{
			if (Control != null && keyPath == "bounds")
				Control.DrawCustomBorder (_borderColor, 1f);
		}
	}
}


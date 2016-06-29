using System;
using Xamarin.Forms;
using BodyReportMobile.Presenter.Framework.Controls;
using BodyReport.iOS;
using Xamarin.Forms.Platform.iOS;
using BodyReportMobile.Presenter;
using UIKit;
using Foundation;

[assembly: ExportRenderer(typeof(TimePicker), typeof(BodyReport.iOS.Framework.Renderers.TimePickerRenderer))]
namespace BodyReport.iOS.Framework.Renderers
{
	public class TimePickerRenderer : Xamarin.Forms.Platform.iOS.TimePickerRenderer
	{
		UIColor _borderColor = UIColor.Gray;
		bool _disposed = false;
		bool _observerPresent = false;
		IntPtr tokenObserveBound = (IntPtr)1;

		public TimePickerRenderer()
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

		protected override void OnElementChanged (ElementChangedEventArgs<TimePicker> e)
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


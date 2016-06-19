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
		IDisposable _boundObserver = null;

		public DatePickerRenderer()
		{
			Color color;
			if (App.GetXamlResources("entryBorderColor", out color))
				_borderColor = color.ToUIColor ();
		}

		protected override void Dispose (bool disposing)
		{
			_disposed = true;
			if (_boundObserver != null)
			{
				_boundObserver.Dispose ();
				_boundObserver = null;
			}
			base.Dispose (disposing);
		}

		protected override void OnElementChanged (ElementChangedEventArgs<DatePicker> e)
		{
			base.OnElementChanged (e);

			if (Control != null) {
				this.Control.BorderStyle = UITextBorderStyle.None;
				if(!_disposed && Control.Layer != null && _boundObserver == null)
					_boundObserver = Control.Layer.AddObserver ("bounds", NSKeyValueObservingOptions.New, obs => ObserveBoundValue(obs));
			}
		}

		public void ObserveBoundValue (NSObservedChange obs)
		{
			if (Control != null)
				Control.DrawCustomBorder (_borderColor, 1f);
		}
	}
}


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
		IDisposable _boundObserver = null;

		public TimePickerRenderer()
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

		protected override void OnElementChanged (ElementChangedEventArgs<TimePicker> e)
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


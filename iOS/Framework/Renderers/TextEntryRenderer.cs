using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using UIKit;
using BodyReportMobile.Presenter;
using Foundation;

[assembly:ExportRenderer (typeof(BodyReportMobile.Presenter.Framework.Controls.TextEntry), typeof(BodyReport.iOS.Framework.Renderers.TextEntryRenderer))]
namespace BodyReport.iOS.Framework.Renderers
{
	public class TextEntryRenderer : EntryRenderer
	{
		UIColor _borderColor = UIColor.Gray;
		bool _disposed = false;
		bool _observerPresent = false;
		IntPtr tokenObserveBound = (IntPtr)1;

		public TextEntryRenderer()
		{
			Color color;
			if (App.GetXamlResources("entryBorderColor", out color))
				_borderColor = color.ToUIColor ();
		}

		protected override void Dispose (bool disposing)
		{
			_disposed = true;

			if (_observerPresent && Control != null && Control.Layer != null)
			{
				Control.Layer.RemoveObserver (this, "bounds", tokenObserveBound);
				_observerPresent = false;
			}
			
			base.Dispose (disposing);
		}

		protected override void OnElementChanged (ElementChangedEventArgs<Entry> e)
		{
			base.OnElementChanged (e);

			if (Control != null) {
				Control.SpellCheckingType = UITextSpellCheckingType.No;             // No Spellchecking
				Control.AutocorrectionType = UITextAutocorrectionType.No;           // No Autocorrection
				Control.AutocapitalizationType = UITextAutocapitalizationType.None; // No Autocapitalization
				Control.BorderStyle = UITextBorderStyle.None;

				if (!_disposed && Control.Layer != null) {
					Control.Layer.AddObserver (this, (NSString)"bounds", NSKeyValueObservingOptions.Old | NSKeyValueObservingOptions.New, tokenObserveBound);
					_observerPresent = true;
				}
			}
		}

		public override void ObserveValue (NSString keyPath, NSObject ofObject,
				   NSDictionary change, IntPtr context)
		{
			if (Control != null && keyPath == "bounds")
				Control.DrawCustomBorder (_borderColor, 1f);

            base.ObserveValue (keyPath, ofObject, change, context);
        }
	}
}


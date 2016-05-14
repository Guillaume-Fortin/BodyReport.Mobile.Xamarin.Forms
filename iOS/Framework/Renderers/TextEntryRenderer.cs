using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using UIKit;

[assembly:ExportRenderer (typeof(BodyReportMobile.Presenter.Framework.Controls.TextEntry), typeof(BodyReport.iOS.Framework.Renderers.TextEntryRenderer))]
namespace BodyReport.iOS.Framework.Renderers
{
	public class TextEntryRenderer : EntryRenderer
	{
		protected override void OnElementChanged (ElementChangedEventArgs<Entry> e)
		{
			base.OnElementChanged (e);

			if (Control != null) {
				Control.SpellCheckingType = UITextSpellCheckingType.No;             // No Spellchecking
				Control.AutocorrectionType = UITextAutocorrectionType.No;           // No Autocorrection
				Control.AutocapitalizationType = UITextAutocapitalizationType.None; // No Autocapitalization
				Control.BorderStyle = UITextBorderStyle.None;
			}
		}
	}
}


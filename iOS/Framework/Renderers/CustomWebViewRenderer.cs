using System.IO;
using System.Net;
using BodyReport.iOS;
using BodyReportMobile.Presenter.Framework.Controls;
using Foundation;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer (typeof (CustomWebView), typeof (CustomWebViewRenderer))]
namespace BodyReport.iOS
{
	public class CustomWebViewRenderer : ViewRenderer<CustomWebView, UIWebView>
	{
		protected override void OnElementChanged (ElementChangedEventArgs<CustomWebView> e)
		{
			base.OnElementChanged (e);

			if (Control == null) {
				SetNativeControl (new UIWebView ());
			}
			if (e.OldElement != null) {
				// Cleanup
			}
			if (e.NewElement != null) {
				((CustomWebView)this.Element).PlatformControl = this.Control;
				var customWebView = Element as CustomWebView;
				string fileName = customWebView.Uri;
				Control.LoadRequest (new NSUrlRequest (new NSUrl (fileName)));
				Control.ScalesPageToFit = true;
			}
		}
	}
}

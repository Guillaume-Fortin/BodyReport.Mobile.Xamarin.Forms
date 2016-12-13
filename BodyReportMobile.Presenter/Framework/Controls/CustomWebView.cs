using System;
using Xamarin.Forms;

namespace BodyReportMobile.Presenter.Framework.Controls
{
	public class CustomWebView : WebView
	{
		public Object PlatformControl { get; set; }

		public static readonly BindableProperty UriProperty = BindableProperty.Create (propertyName: "Uri",
				returnType: typeof (string),
				declaringType: typeof (CustomWebView),
				defaultValue: default (string));

		public string Uri {
			get { return (string)GetValue (UriProperty); }
			set { SetValue (UriProperty, value); }
		}
	}
}

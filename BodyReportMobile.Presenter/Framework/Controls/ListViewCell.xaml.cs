using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BodyReportMobile.Presenter.Framework.Controls
{
	[XamlCompilation (XamlCompilationOptions.Compile)]
	public partial class ListViewCell : ViewCell
	{
		/*public static readonly BindableProperty TextProperty = BindableProperty.Create ("Text", typeof(string), typeof(ListViewCell), "");

		public string Text {
			get { return (string)GetValue (TextProperty); }
			set { SetValue (TextProperty, value); }
		}

		public static readonly BindableProperty DetailProperty = BindableProperty.Create ("Detail", typeof(string), typeof(ListViewCell), "");

		public string Detail {
			get { return (string)GetValue (DetailProperty); }
			set { SetValue (DetailProperty, value); }
		}

		public static readonly BindableProperty ImageSourceProperty = BindableProperty.Create ("ImageSource", typeof(string), typeof(ListViewCell), "");

		public string ImageSource {
			get { return (string)GetValue (ImageSourceProperty); }
			set { SetValue (ImageSourceProperty, value); }
		}

		public static readonly BindableProperty TextColorProperty = BindableProperty.Create ("TextColor", typeof(Color), typeof(ListViewCell), Color.Black);

		public Color TextColor {
			get { return (Color)GetValue (TextColorProperty); }
			set { SetValue (TextColorProperty, value); }
		}

		public static readonly BindableProperty DetailTextColorProperty = BindableProperty.Create ("DetailTextColor", typeof(Color), typeof(ListViewCell), Color.Black);

		public Color DetailTextColor {
			get { return (Color)GetValue (DetailTextColorProperty); }
			set { SetValue (DetailTextColorProperty, value); }
		}

		public static readonly BindableProperty BackgroundColorProperty = BindableProperty.Create ("BackgroundColor", typeof(Color), typeof(ListViewCell), Color.White);

		public Color BackgroundColor {
			get { return (Color)GetValue (BackgroundColorProperty); }
			set { SetValue (BackgroundColorProperty, value); }
		}*/

		public ListViewCell ()
		{
			InitializeComponent ();
		}
	}
}


using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace BodyReportMobile.Presenter.Framework.Controls
{
	public partial class TouchViewCell : ViewCell
	{
		public static readonly BindableProperty TitleProperty = BindableProperty.Create ("Title", typeof(string), typeof(TouchViewCell), "", BindingMode.TwoWay);

		public string Title {
			get { return (string)GetValue (TitleProperty); }
			set { SetValue (TitleProperty, value); }
		}

		public static readonly BindableProperty ValueProperty = BindableProperty.Create ("Value", typeof(string), typeof(TouchViewCell), "", BindingMode.TwoWay);

		public string Value {
			get { return (string)GetValue (ValueProperty); }
			set { SetValue (ValueProperty, value); }
		}

		public static readonly BindableProperty DescriptionProperty = BindableProperty.Create ("Description", typeof(string), typeof(TouchViewCell), "", BindingMode.TwoWay);

		public string Description {
			get { return (string)GetValue (DescriptionProperty); }
			set { SetValue (DescriptionProperty, value); }
		}

		public static readonly BindableProperty IsIndicatorVisibleProperty = BindableProperty.Create ("IsIndicatorVisible", typeof(bool), typeof(TouchViewCell), false, BindingMode.TwoWay);

		public bool IsIndicatorVisible {
			get { return (bool)GetValue (IsIndicatorVisibleProperty); }
			set { SetValue (IsIndicatorVisibleProperty, value); }
		}

		public static readonly BindableProperty ImageProperty = BindableProperty.Create ("Image", typeof(string), typeof(TouchViewCell), "", BindingMode.TwoWay);

		public string Image {
			get { return (string)GetValue (ImageProperty); }
			set { SetValue (ImageProperty, value); }
		}

		public static readonly BindableProperty IsImageVisibleProperty = BindableProperty.Create ("IsImageVisible", typeof(bool), typeof(TouchViewCell), false, BindingMode.TwoWay);

		public bool IsImageVisible {
			get { return (bool)GetValue (IsImageVisibleProperty); }
			set { SetValue (IsImageVisibleProperty, value); }
		}

        public static readonly BindableProperty BackgroundColorProperty = BindableProperty.Create("Title", typeof(Color), typeof(TouchViewCell), Color.Default, BindingMode.TwoWay);

        public Color BackgroundColor
        {
            get { return (Color)GetValue(BackgroundColorProperty); }
            set { SetValue(BackgroundColorProperty, value); }
        }

        public TouchViewCell ()
		{
			InitializeComponent ();

        /*	this.TitleLabel.SetBinding (Label.TextProperty, new Binding(path: "Title", source: this));
			this.TitleLabel.SetBinding (Label.TextProperty, new Binding(path: "Title", source: this));*/
        }
	}
}


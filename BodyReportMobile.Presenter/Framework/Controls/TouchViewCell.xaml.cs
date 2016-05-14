using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BodyReportMobile.Presenter.Framework.Controls
{
	[XamlCompilation (XamlCompilationOptions.Compile)]
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
        
        public static readonly BindableProperty IsCheckedVisibleProperty = BindableProperty.Create("IsCheckedVisible", typeof(bool), typeof(TouchViewCell), false, BindingMode.TwoWay);

        public bool IsCheckedVisible
        {
            get { return (bool)GetValue(IsCheckedVisibleProperty); }
            set { SetValue(IsCheckedVisibleProperty, value); }
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

        public static readonly BindableProperty ImageWidthRequestProperty = BindableProperty.Create("ImageWidthRequest", typeof(int), typeof(TouchViewCell), 40, BindingMode.TwoWay);

        public int ImageWidthRequest
        {
            get { return (int)GetValue(ImageWidthRequestProperty); }
            set { SetValue(ImageWidthRequestProperty, value); }
        }

        public static readonly BindableProperty ImageHeightRequestProperty = BindableProperty.Create("ImageHeightRequest", typeof(int), typeof(TouchViewCell), 40, BindingMode.TwoWay);

        public int ImageHeightRequest
        {
            get { return (int)GetValue(ImageHeightRequestProperty); }
            set { SetValue(ImageHeightRequestProperty, value); }
        }

        public static readonly BindableProperty IsImageVisibleProperty = BindableProperty.Create ("IsImageVisible", typeof(bool), typeof(TouchViewCell), false, BindingMode.TwoWay);

		public bool IsImageVisible {
			get { return (bool)GetValue (IsImageVisibleProperty); }
			set { SetValue (IsImageVisibleProperty, value); }
		}

        public static readonly BindableProperty BackgroundColorProperty = BindableProperty.Create("BackgroundColor", typeof(Color), typeof(TouchViewCell), Color.Default, BindingMode.TwoWay);

        public Color BackgroundColor
        {
            get { return (Color)GetValue(BackgroundColorProperty); }
            set { SetValue(BackgroundColorProperty, value); }
        }

        public static readonly BindableProperty TitleTextColorProperty = BindableProperty.Create("TitleTextColor", typeof(Color), typeof(TouchViewCell), Color.Default, BindingMode.TwoWay);

        public Color TitleTextColor
        {
            get { return (Color)GetValue(TitleTextColorProperty); }
            set { SetValue(TitleTextColorProperty, value); }
        }

        public static readonly BindableProperty ValueTextColorProperty = BindableProperty.Create("ValueTextColor", typeof(Color), typeof(TouchViewCell), Color.Default, BindingMode.TwoWay);

        public Color ValueTextColor
        {
            get { return (Color)GetValue(ValueTextColorProperty); }
            set { SetValue(ValueTextColorProperty, value); }
        }

        public static readonly BindableProperty DescriptionTextColorProperty = BindableProperty.Create("DescriptionTextColor", typeof(Color), typeof(TouchViewCell), Color.Default, BindingMode.TwoWay);

        public Color TextColor
        {
            get { return (Color)GetValue(DescriptionTextColorProperty); }
            set { SetValue(DescriptionTextColorProperty, value); }
        }

        public TouchViewCell ()
		{
			InitializeComponent ();

            //ValueLabel.Triggers.Add()

        /*	this.TitleLabel.SetBinding (Label.TextProperty, new Binding(path: "Title", source: this));
			this.TitleLabel.SetBinding (Label.TextProperty, new Binding(path: "Title", source: this));*/
        }

        public void SetTitleTrigger(TriggerBase trigger)
        {
            TitleLabel.Triggers.Add(trigger);
        }

        public void SetValueTrigger(TriggerBase trigger)
        {
            ValueLabel.Triggers.Add(trigger);
        }

        public void SetDescriptionTrigger(TriggerBase trigger)
        {
            DescriptionLabel.Triggers.Add(trigger);
        }
    }
}


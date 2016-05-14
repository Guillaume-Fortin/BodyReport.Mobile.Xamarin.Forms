using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BodyReportMobile.Presenter.Framework.Controls
{
	[XamlCompilation (XamlCompilationOptions.Compile)]
	public partial class NumericEntryCell : ViewCell
	{
		public static readonly BindableProperty LabelProperty = BindableProperty.Create ("Label", typeof(string), typeof(TouchViewCell), "");

		public string Label {
			get { return (string)GetValue (LabelProperty); }
			set { SetValue (LabelProperty, value); }
		}

		public static readonly BindableProperty TextProperty = BindableProperty.Create ("Text", typeof(string), typeof(TouchViewCell), "");

		public string Text {
			get { return (string)GetValue (TextProperty); }
			set { SetValue (TextProperty, value); }
		}

		public static readonly BindableProperty MinValueProperty = BindableProperty.Create ("MinValue", typeof(double), typeof(NumericEntry), double.MinValue);

		public double MinValue {
			get { return (double)GetValue (MinValueProperty); }
			set { SetValue (MinValueProperty, value); }
		}

		public static readonly BindableProperty MaxValueProperty = BindableProperty.Create ("MaxValue", typeof(double), typeof(NumericEntry), double.MaxValue);

		public double MaxValue {
			get { return (double)GetValue (MaxValueProperty); }
			set { SetValue (MaxValueProperty, value); }
		}

		public static readonly BindableProperty IsIntegerProperty = BindableProperty.Create ("IsInteger", typeof(bool), typeof(NumericEntry), false);

		public bool IsInteger {
			get { return (bool)GetValue (IsIntegerProperty); }
			set { SetValue (IsIntegerProperty, value); }
		}

		public NumericEntryCell ()
		{
			InitializeComponent ();
		}
	}
}


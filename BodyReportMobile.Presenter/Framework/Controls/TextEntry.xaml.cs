using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BodyReportMobile.Presenter.Framework.Controls
{
	[XamlCompilation (XamlCompilationOptions.Compile)]
	public partial class TextEntry : Entry
	{
		public static readonly BindableProperty MaxLengthProperty = BindableProperty.Create ("MaxLength", typeof(int), typeof(TextEntry), -1);

		public int MaxLength
        {
			get { return (int)GetValue (MaxLengthProperty); }
			set { SetValue (MaxLengthProperty, value); }
		}

		public TextEntry()
		{
			InitializeComponent ();
		}
	}
}


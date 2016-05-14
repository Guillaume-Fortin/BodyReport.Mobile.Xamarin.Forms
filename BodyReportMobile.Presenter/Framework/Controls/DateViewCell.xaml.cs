using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BodyReportMobile.Presenter.Framework.Controls
{
	[XamlCompilation (XamlCompilationOptions.Compile)]
    public partial class DateViewCell : ViewCell
    {
        public static readonly BindableProperty TitleProperty = BindableProperty.Create("Title", typeof(string), typeof(DateViewCell), "", BindingMode.TwoWay);

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public static readonly BindableProperty DateProperty = BindableProperty.Create("Date", typeof(DateTime), typeof(DateViewCell), DateTime.Now, BindingMode.TwoWay);

        public DateTime Date
        {
            get { return (DateTime)GetValue(DateProperty); }
            set { SetValue(DateProperty, value); }
        }

        public DateViewCell()
        {
            InitializeComponent();
        }
    }
}

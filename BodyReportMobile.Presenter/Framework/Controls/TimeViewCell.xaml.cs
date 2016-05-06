using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace BodyReportMobile.Presenter.Framework.Controls
{
    public partial class TimeViewCell : ViewCell
    {
        public static readonly BindableProperty TitleProperty = BindableProperty.Create("Title", typeof(string), typeof(TimeViewCell), "", BindingMode.TwoWay);

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public static readonly BindableProperty TimeProperty = BindableProperty.Create("Time", typeof(TimeSpan), typeof(TimeViewCell), TimeSpan.Zero, BindingMode.TwoWay);

        public TimeSpan Time
        {
            get { return (TimeSpan)GetValue(TimeProperty); }
            set { SetValue(TimeProperty, value); }
        }

        public TimeViewCell()
        {
            InitializeComponent();
        }
    }
}

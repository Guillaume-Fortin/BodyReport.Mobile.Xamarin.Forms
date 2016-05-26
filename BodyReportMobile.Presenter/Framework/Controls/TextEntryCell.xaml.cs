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
    public partial class TextEntryCell : ViewCell
    {
        public static readonly BindableProperty LabelProperty = BindableProperty.Create("Label", typeof(string), typeof(TextEntryCell), "", BindingMode.TwoWay);

        public string Label
        {
            get { return (string)GetValue(LabelProperty); }
            set { SetValue(LabelProperty, value); }
        }

        public static readonly BindableProperty TextProperty = BindableProperty.Create("Text", typeof(string), typeof(TextEntryCell), "", BindingMode.TwoWay);

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly BindableProperty MaxLengthProperty = BindableProperty.Create("MaxLength", typeof(int), typeof(TextEntryCell), 0, BindingMode.TwoWay);

        public int MaxLength
        {
            get { return (int)GetValue(MaxLengthProperty); }
            set
            {
                SetValue(MaxLengthProperty, value);
            }
        }

        public TextEntryCell()
        {
            InitializeComponent();
        }
    }
}

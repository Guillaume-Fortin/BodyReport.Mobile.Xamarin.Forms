using BodyReportMobile.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace BodyReportMobile.Presenter.Pages
{
    public partial class RegisterAccountPage : BaseContentPage
    {
        public RegisterAccountPage(RegisterAccountViewModel baseViewModel) : base(baseViewModel)
        {
            InitializeComponent();
        }
    }
}

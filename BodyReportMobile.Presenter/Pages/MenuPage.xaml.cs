using BodyReportMobile.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace BodyReportMobile.Presenter.Pages
{
    public partial class MenuPage : BaseContentPage
    {
        public MenuPage(MenuViewModel menuViewModel) : base(menuViewModel)
        {
            InitializeComponent();
        }

        public void OnCellTapped(object sender, EventArgs e)
        {
            var viewModel = BindingContext as MenuViewModel;
            if (sender == EditUserProfileCell)
                viewModel.EditUserProfileCommand.Execute(null);
            else if (sender == ChangeLanguageCell)
                viewModel.ChangeLanguageCommand.Execute(null);
        }
    }
}

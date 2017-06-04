using BodyReportMobile.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BodyReportMobile.Presenter.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
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
            else if (sender == LogOffCell)
                viewModel.LogOffCommand.Execute(null);
            else if (sender == ChangeLanguageCell)
                viewModel.ChangeLanguageCommand.Execute(null);
            else if (sender == ConfidentialityRulesCell)
                viewModel.DisplayConfidentialityRulesCommand.Execute(null);
        }
    }
}

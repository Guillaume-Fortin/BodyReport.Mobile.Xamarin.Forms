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
    public partial class EditUserProfilePage : BaseContentPage
    {
        public EditUserProfilePage(EditUserProfileViewModel viewModel) : base(viewModel)
        {
            InitializeComponent();
        }

        public void OnCellTapped(object sender, EventArgs e)
        {
            var viewModel = BindingContext as EditUserProfileViewModel;
            if (viewModel == null)
                return;

            if (sender == SexCell)
                viewModel.ChangeSexCommand.Execute(null);
            else if (sender == UnitCell)
                viewModel.ChangeUnitCommand.Execute(null);
            else if (sender == CountryCell)
                viewModel.ChangeCountryCommand.Execute(null);
        }
    }
}

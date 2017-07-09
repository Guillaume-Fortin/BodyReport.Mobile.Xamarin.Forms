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
	[XamlCompilation (XamlCompilationOptions.Compile)]
    public partial class CreateTrainingDayPage : BaseContentPage
    {
        public CreateTrainingDayPage(CreateTrainingDayViewModel viewModel) : base(viewModel)
        {
            InitializeComponent();
            
            CreateComponent(viewModel);
        }

        private void CreateComponent(CreateTrainingDayViewModel viewModel)
        {
            if (viewModel != null && viewModel.EditMode == BodyReport.Message.TEditMode.Edit)
            {
                SwitchCell convertionSwitchCell = new SwitchCell();
                convertionSwitchCell.BindingContext = viewModel;
                convertionSwitchCell.SetBinding(SwitchCell.TextProperty, (CreateTrainingDayViewModel source) => source.AutomaticalUnitConvertionLabel, mode: BindingMode.OneWay);
                convertionSwitchCell.SetBinding(SwitchCell.OnProperty, (CreateTrainingDayViewModel source) => source.BindingTrainingDay.AutomaticalUnitConvertion, mode: BindingMode.TwoWay);
                tableSection.Add(convertionSwitchCell);
            }
        }

        public void OnCellTapped(object sender, EventArgs e)
        {
            var viewModel = BindingContext as CreateTrainingDayViewModel;
            if (viewModel == null)
                return;

            if (sender == UnitCell)
                viewModel.ChangeUnitCommand.Execute(null);
        }
    }
}

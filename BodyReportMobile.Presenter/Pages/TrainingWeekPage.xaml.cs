using BodyReportMobile.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace BodyReportMobile.Presenter.Pages
{
    public partial class TrainingWeekPage : BaseContentPage
    {
        public TrainingWeekPage(TrainingWeekViewModel baseViewModel) : base(baseViewModel)
        {
            InitializeComponent();
        }

        public void OnCellTapped(object sender, EventArgs e)
        {
            var viewModel = BindingContext as TrainingWeekViewModel;
            if (sender == MondayCell)
                viewModel.ViewTrainingDayCommand.Execute(DayOfWeek.Monday);
            else if (sender == TuesdayCell)
                viewModel.ViewTrainingDayCommand.Execute(DayOfWeek.Tuesday);
            else if (sender == WednesdayCell)
                viewModel.ViewTrainingDayCommand.Execute(DayOfWeek.Wednesday);
            else if (sender == ThursdayCell)
                viewModel.ViewTrainingDayCommand.Execute(DayOfWeek.Thursday);
            else if (sender == FridayCell)
                viewModel.ViewTrainingDayCommand.Execute(DayOfWeek.Friday);
            else if (sender == SaturdayCell)
                viewModel.ViewTrainingDayCommand.Execute(DayOfWeek.Saturday);
            else if (sender == SundayCell)
                viewModel.ViewTrainingDayCommand.Execute(DayOfWeek.Sunday);
        }
    }
}

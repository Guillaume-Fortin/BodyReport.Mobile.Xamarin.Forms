using BodyReportMobile.Core.Framework.Binding;
using BodyReportMobile.Core.ViewModels;
using BodyReportMobile.Presenter.Framework.Controls;
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
    public partial class TrainingWeekPage : BaseContentPage
    {
        public TrainingWeekPage(TrainingWeekViewModel baseViewModel) : base(baseViewModel)
        {
            InitializeComponent();

            baseViewModel.PropertyChanged += TrainingWeekPage_PropertyChanged;
        }

        private void TrainingWeekPage_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "BindingWeekTrainingDays")
            {
                TrainingWeekViewModel viewModel = sender as TrainingWeekViewModel;
                if (viewModel != null)
                {
                    TouchViewCell touchViewCell;
                    DaySection.Clear();
                    foreach (var bindingWeekTrainingDay in viewModel.BindingWeekTrainingDays)
                    {
                        touchViewCell = new TouchViewCell()
                        {
                            IsIndicatorVisible = true,
                            BindingContext = bindingWeekTrainingDay,
                            TitleTextColor = Color.Red,
                            ValueTextColor = Color.Red
                        };
                        touchViewCell.Tapped += DayCellTaped;
                        touchViewCell.SetBinding(TouchViewCell.ValueProperty, (BindingWeekTrainingDay source) => source.Label);

                        var trigger = new DataTrigger(typeof(Label));
                        trigger.BindingContext = bindingWeekTrainingDay;
                        trigger.Binding = new Binding("TrainingDayExist");
                        trigger.Value = true;
                        var setter = new Setter();
                        setter.Property = Label.TextColorProperty;
                        setter.Value = Color.FromHex("#337ab7");
                        trigger.Setters.Add(setter);
                        touchViewCell.SetValueTrigger(trigger);

                        trigger = new DataTrigger(typeof(Label));
                        trigger.BindingContext = bindingWeekTrainingDay;
                        trigger.Binding = new Binding("TrainingDayExist");
                        trigger.Value = true;
                        setter = new Setter();
                        setter.Property = Label.TextColorProperty;
                        setter.Value = Color.FromHex("#337ab7");
                        trigger.Setters.Add(setter);
                        touchViewCell.SetTitleTrigger(trigger);

                        var menuItem = new MenuItem();
                        menuItem.SetBinding(MenuItem.TextProperty, new Binding(path: "SwitchDayLabel", source: viewModel));
                        menuItem.BindingContext = bindingWeekTrainingDay;
                        menuItem.Command = viewModel.SwitchTrainingDayCommand;
                        menuItem.SetBinding(MenuItem.CommandParameterProperty, new Binding(path: "DayOfWeek", source: bindingWeekTrainingDay));
                        touchViewCell.ContextActions.Add(menuItem);

                        DaySection.Add(touchViewCell);
                    }   
                }
            }
        }

        public void DayCellTaped(object sender, EventArgs e)
        {
            if (sender != null)
            {
                var bindingWeekTrainingDay = (sender as TouchViewCell).BindingContext as BindingWeekTrainingDay;
                if(bindingWeekTrainingDay != null)
                {
                    (_viewModel as TrainingWeekViewModel).ViewTrainingDayCommand.Execute(bindingWeekTrainingDay.DayOfWeek);
                }
            }
        }
    }
}

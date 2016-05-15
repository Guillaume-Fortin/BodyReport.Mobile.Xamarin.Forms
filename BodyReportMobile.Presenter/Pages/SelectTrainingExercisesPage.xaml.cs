using BodyReportMobile.Core.Message.Binding;
using BodyReportMobile.Core.ViewModels;
using BodyReportMobile.Presenter.Framework.Controls;
using BodyReportMobile.Presenter.Framework.Converter;
using Message;
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
    public partial class SelectTrainingExercisesPage : BaseContentPage
    {
        public SelectTrainingExercisesPage(SelectTrainingExercisesViewModel baseViewModel) : base(baseViewModel)
        {
            InitializeComponent();

            baseViewModel.PropertyChanged += BaseViewModel_PropertyChanged;
        }

        private void BaseViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "BindingBodyExercises")
            {
                SelectTrainingExercisesViewModel viewModel = sender as SelectTrainingExercisesViewModel;
                if (viewModel != null)
                {
                    TouchViewCell touchViewCell;
                    BodyExerciseSection.Clear();
                    foreach (var bindingBodyExercise in viewModel.BindingBodyExercises)
                    {
                        touchViewCell = new TouchViewCell()
                        {
                            IsIndicatorVisible = false,
                            BindingContext = bindingBodyExercise,
                            ImageWidthRequest = 140,
                            ImageHeightRequest = 140
                        };
                        touchViewCell.Tapped += BodyExerciseItemTapped;
                        touchViewCell.SetBinding(TouchViewCell.IsCheckedVisibleProperty, (BindingBodyExercise source) => source.Selected);
                        touchViewCell.SetBinding(TouchViewCell.ImageProperty, (BindingBodyExercise source) => source.Image);
                        touchViewCell.SetBinding(TouchViewCell.ValueProperty, (BindingBodyExercise source) => source.Name);

                        BodyExerciseSection.Add(touchViewCell);
                    }
                }
            }
        }
        
        private void ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null) return;
            ((ListView)sender).SelectedItem = null; // de-select the row
        }

        public void MuscularGroupItemTapped(object sender, EventArgs e)
        {
            var selectTrainingExercisesViewModel = _viewModel as SelectTrainingExercisesViewModel;
            if(selectTrainingExercisesViewModel != null)
            {
                selectTrainingExercisesViewModel.SelectMuscularGroupCommand.Execute(null);
            }
        }

        public void MuscleItemTapped(object sender, EventArgs e)
        {
            var selectTrainingExercisesViewModel = _viewModel as SelectTrainingExercisesViewModel;
            if (selectTrainingExercisesViewModel != null)
            {
                selectTrainingExercisesViewModel.SelectMuscleCommand.Execute(null);
            }
        }

        public void BodyExerciseItemTapped(object sender, EventArgs e)
        {
            TouchViewCell touchViewCell = sender as TouchViewCell;
            if(touchViewCell != null && touchViewCell.BindingContext != null)
            {
                var bindingBodyExercise = touchViewCell.BindingContext as BindingBodyExercise;
                var selectTrainingExercisesViewModel = _viewModel as SelectTrainingExercisesViewModel;
                if (selectTrainingExercisesViewModel != null)
                {
                    selectTrainingExercisesViewModel.SelectBodyExerciseCommand.Execute(bindingBodyExercise);
                }
            }
        }
    }
}

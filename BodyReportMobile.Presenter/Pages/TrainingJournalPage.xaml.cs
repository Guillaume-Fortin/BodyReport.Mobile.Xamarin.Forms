using BodyReportMobile.Core.ViewModels;
using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace BodyReportMobile.Presenter.Pages
{
	public partial class TrainingJournalPage : BaseContentPage
	{
		public TrainingJournalPage (TrainingJournalViewModel baseViewModel) : base(baseViewModel)
        {
			InitializeComponent ();
		}

        private void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null || (sender as ListView) == null)
                return;

            var selectItem = e.SelectedItem;
            (sender as ListView).SelectedItem = null; // necessary for reselect item
            (BindingContext as TrainingJournalViewModel).ViewTrainingWeekCommand.Execute(selectItem);
        }
    }
}


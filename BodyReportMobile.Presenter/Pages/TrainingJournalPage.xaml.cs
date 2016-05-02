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
            if (e.SelectedItem == null)
            {
                return;
            }

            (BindingContext as TrainingJournalViewModel).ViewTrainingWeekCommand.Execute(null);
        }
    }
}


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
	}
}


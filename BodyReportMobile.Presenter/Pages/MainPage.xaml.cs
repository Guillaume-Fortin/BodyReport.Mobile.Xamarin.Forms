using BodyReportMobile.Core.ViewModels;
using System;

using Xamarin.Forms;

namespace BodyReportMobile.Presenter.Pages
{
	public partial class MainPage : BaseContentPage
	{
		public MainPage () : base ()
		{
			DisableBackButton = true;
			InitializeComponent ();
		}

		public void OnCellTapped (object sender, EventArgs e)
		{
			var viewModel = BindingContext as MainViewModel;
			//viewModel.DisplayYearCommand.Execute ();
			if (sender == TrainingJournalCell)
				viewModel.GoToTrainingJournalCommand.Execute (null);
			else if (sender == ChangeLanguageCell)
				viewModel.GoToChangeLanguageCommand.Execute (null);
		}
	}
}


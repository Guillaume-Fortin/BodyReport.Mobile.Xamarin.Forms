using BodyReportMobile.Core.ViewModels;
using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BodyReportMobile.Presenter.Pages
{
	[XamlCompilation (XamlCompilationOptions.Compile)]
	public partial class MainPage : BaseContentPage
	{
		public MainPage (MainViewModel viewModel) : base (viewModel)
		{
			DisableBackButton = true;
			InitializeComponent ();
		}

		public void OnCellTapped (object sender, EventArgs e)
		{
            var viewModel = BindingContext as MainViewModel;
			if (sender == TrainingJournalCell)
				viewModel.GoToTrainingJournalCommand.Execute (null);
		}
	}
}


using System;
using System.Collections.Generic;

using Xamarin.Forms;
using System.Globalization;
using System.Text;
using System.Diagnostics;
using BodyReportMobile.Core.ViewModels;

namespace BodyReportMobile.Presenter.Pages
{
	public partial class EditTrainingWeekPage : BaseContentPage
	{
		public EditTrainingWeekPage (BaseViewModel viewModel) : base(viewModel)
		{
			InitializeComponent ();
		}

		public void OnCellTapped(object sender, EventArgs e)
		{
			var viewModel = BindingContext as EditTrainingWeekViewModel;
			//viewModel.DisplayYearCommand.Execute ();
			if (sender == YearCell) {
				viewModel.ChangeYearCommand.Execute (null);
				/*var answer = await DisplayAlert ("Question?", "Would you like to play a game", "Yes", "No");
				Debug.WriteLine ("Answer: " + answer);
				YearText.Text = "2014";
				*/
			} else if (sender == WeekOfYearCell)
				viewModel.ChangeWeekOfYearCommand.Execute (null);
				
		}
	}
}


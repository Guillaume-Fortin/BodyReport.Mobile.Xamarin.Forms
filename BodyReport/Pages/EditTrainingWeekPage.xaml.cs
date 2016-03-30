using System;
using System.Collections.Generic;

using Xamarin.Forms;
using MvvmCross.Platform.Converters;
using System.Globalization;
using System.Text;
using BodyReportMobile.Core;
using System.Diagnostics;

namespace BodyReport
{
	public partial class EditTrainingWeekPage : BaseContentPage
	{
		public EditTrainingWeekPage () : base()
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


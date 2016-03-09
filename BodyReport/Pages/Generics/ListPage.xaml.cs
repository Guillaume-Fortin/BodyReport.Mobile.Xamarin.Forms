using System;
using System.Collections.Generic;

using Xamarin.Forms;
using BodyReportMobile.Core;

namespace BodyReport
{
	public partial class ListPage : BaseContentPage
	{
		public ListPage ()
		{
			InitializeComponent ();
		}

		private void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
		{
			if (e.SelectedItem == null) {
				return;
			}

			(BindingContext as ListViewModel).ValidateCommand.Execute (null);
		}
	}
}


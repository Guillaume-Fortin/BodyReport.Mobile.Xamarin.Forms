using System;
using System.Collections.Generic;
using System.Linq;

using Xamarin.Forms;
using BodyReportMobile.Core;
using BodyReportMobile.Core.ViewModels.Generic;
using BodyReportMobile.Core.ViewModels;

namespace BodyReportMobile.Presenter.Pages.Generics
{
    public partial class ListPage : BaseContentPage
	{
		public ListPage (ListViewModel baseViewModel) : base(baseViewModel)
		{
			InitializeComponent ();
		}

		protected override void OnAppearing ()
		{
			base.OnAppearing ();

			var datas = (BindingContext as ListViewModel).Datas;
			if (datas != null) {
				var selectedItem = datas.Where (d => d != null && d.IsSelected).FirstOrDefault ();
                //if(selectedItem != null)
                //listView.ScrollTo (selectedItem, ScrollToPosition.Center, true);
            }

            /*var selectItem = (BindingContext as ListViewModel).SelectedItem;
			listView.SelectedItem = selectItem;
			if (listView.SelectedItem != null)
				listView.ScrollTo (listView.SelectedItem, ScrollToPosition.Center, false);*/
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


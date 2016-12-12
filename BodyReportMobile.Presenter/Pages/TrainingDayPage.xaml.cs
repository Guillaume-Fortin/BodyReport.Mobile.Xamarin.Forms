using BodyReportMobile.Core.ViewModels;
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
    public partial class TrainingDayPage : BaseContentPage
    {
        public TrainingDayPage(TrainingDayViewModel viewModel) : base(viewModel)
        {
            InitializeComponent();
			if (this.ToolbarItems != null && Device.OS == TargetPlatform.iOS)
			{
				for (int i = 0; i < ToolbarItems.Count; i++) {
					if (ToolbarItems[i].Text == viewModel.PrintLabel) {
						this.ToolbarItems.Remove (ToolbarItems[i]);
						break;
					}
				}
			}
        }

        private void ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null) return;
            ((ListView)sender).SelectedItem = null; // de-select the row
        }
    }
}

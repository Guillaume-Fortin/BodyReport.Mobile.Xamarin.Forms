using BodyReportMobile.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace BodyReportMobile.Presenter.Pages
{
    public partial class TrainingDayPage : BaseContentPage
    {
        public TrainingDayPage(TrainingDayViewModel viewModel) : base(viewModel)
        {
            InitializeComponent();
        }

        private void ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null) return;
            ((ListView)sender).SelectedItem = null; // de-select the row
        }
    }
}

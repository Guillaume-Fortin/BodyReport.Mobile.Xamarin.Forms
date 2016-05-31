using BodyReportMobile.Core.ViewModels;
using BodyReportMobile.Presenter.Framework.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BodyReportMobile.Presenter.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditTrainingExercisePage : BaseContentPage
    {
        public EditTrainingExercisePage(EditTrainingExerciseViewModel viewModel) : base(viewModel)
        {
            InitializeComponent();
        }

        private void ListViewItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null) return;
            ((ListView)sender).SelectedItem = null; // de-select the row for disable hightlight
        }
    }
}

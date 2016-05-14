using System;
using System.Windows.Input;
using System.Collections.ObjectModel;
using BodyReportMobile.Core.Message;
using System.Collections.Generic;
using System.Threading.Tasks;
using BodyReportMobile.Core.Framework;
using Xamarin.Forms;

namespace BodyReportMobile.Core.ViewModels.Generic
{
	public class ListViewModel: BaseViewModel
	{
		public class ListViewModelResult
		{
			public bool Validated { get; set; } = false;
			public GenericData SelectedData { get; set; }
		}
        
        private GenericData _defaultSelectedData = null;
        public ObservableCollection<GenericData> Datas { get; set; } = new ObservableCollection<GenericData>();
		public GenericData SelectedItem { get; set; }

        public ListViewModel() : base()
        {
        }

        private void Init()
        {
            try
            {
                SelectItem(_defaultSelectedData);
            }
            catch
            {
                //TODO log
            }
        }

		private void SelectItem(GenericData defaultSelectedData)
		{
			if (Datas != null)
            {
				foreach (var data in Datas)
                {
					if (defaultSelectedData != null && defaultSelectedData == data)
						data.IsSelected = true;
					else
						data.IsSelected = false;
				}
			}
		}

		public static async Task<ListViewModelResult> ShowGenericList(string title, List<GenericData> datas, GenericData defaultSelectedData, BaseViewModel parent = null)
		{
            ListViewModel listViewModel = new ListViewModel();
            listViewModel.ViewModelGuid = Guid.NewGuid().ToString();
            listViewModel._defaultSelectedData = defaultSelectedData;
            listViewModel.TitleLabel = title;
            if (datas != null)
            {
                foreach (var data in datas)
                    listViewModel.Datas.Add(data);
            }
            listViewModel.Init();

            var result = new ListViewModelResult ();
			result.Validated = await ShowModalViewModel(listViewModel, parent);
            result.SelectedData = listViewModel.SelectedItem;

			return result;
		}

		public ICommand ValidateCommand
		{
			get
			{
				return new Command ((genericData) => {
					if (ActionIsInProgress)
                        return;
                    try
                    {
                        ActionIsInProgress = true;
						SelectedItem = genericData as GenericData;
                        if (ValidateViewModel())
                        {
                            CloseViewModel();
                        }
                    }
                    catch
                    {
                    }
                    finally
                    {
                        ActionIsInProgress = false;
                    }
				});
			}
		}

        /// <summary>
        /// Verify item selected
        /// </summary>
        /// <returns></returns>
		private bool ValidateViewModel()
		{
			return SelectedItem != null;
		}
	}
}


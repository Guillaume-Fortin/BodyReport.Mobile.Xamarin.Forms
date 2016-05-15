using System;
using System.Windows.Input;
using System.Collections.ObjectModel;
using BodyReportMobile.Core.Message;
using System.Collections.Generic;
using System.Threading.Tasks;
using BodyReportMobile.Core.Framework;

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

		public static async Task<ListViewModelResult> ShowGenericListAsync(string title, List<GenericData> datas, GenericData defaultSelectedData, BaseViewModel parent = null)
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
			result.Validated = await ShowModalViewModelAsync(listViewModel, parent);
            result.SelectedData = listViewModel.SelectedItem;

			return result;
		}

		public void ValidateAction(GenericData genericData)
        {
			if (genericData == null)
                return;
            try
            {
				SelectedItem = genericData as GenericData;
                if (ValidateViewModel())
                {
                    CloseViewModel();
                }
            }
            catch
            {
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

        #region Command

        public ICommand ValidateCommand
        {
            get
            {
                return new ViewModelCommand(this, (genericData) => {
                    ValidateAction(genericData as GenericData);
                });
            }
        }

        #endregion
    }
}


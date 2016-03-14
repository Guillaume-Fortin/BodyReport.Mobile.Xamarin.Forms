using System;
using BodyReportMobile.Core.ViewModels;
using MvvmCross.Plugins.Messenger;
using System.Windows.Input;
using MvvmCross.Core.ViewModels;
using System.Collections.ObjectModel;
using BodyReportMobile.Core.Message;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BodyReportMobile.Core
{
	public class ListViewModel: BaseViewModel
	{
		public class ListViewModelResult
		{
			public bool ViewModelValidated { get; set; } = false;
			public object SelectedTag { get; set; }
		}

		private static readonly string TITLE = "TITLE";
		private static readonly string DATAS = "DATAS";
		private static readonly string SELECTED_TAG = "SELECTED_TAG";
		private static readonly string OUT_SELECTED_TAG_ITEM = "OUT_SELECTED_TAG_ITEM";

		public ObservableCollection<GenericData> Datas { get; set; } = new ObservableCollection<GenericData>();
		public GenericData SelectedItem { get; set; }
		public string Title { get; set; }

		public ListViewModel (IMvxMessenger messenger) : base(messenger)
		{
		}

		public override void Init(string viewModelGuid, bool autoClearViewModelDataCollection)
		{
			base.Init (viewModelGuid, autoClearViewModelDataCollection);

			Title = ViewModelDataCollection.Get<string> (viewModelGuid, TITLE);
			var datas = ViewModelDataCollection.Get<List<GenericData>> (viewModelGuid, DATAS);
			if (datas != null) {
				foreach (var data in datas)
					Datas.Add (data);
			}
			var defaultSelectedData = ViewModelDataCollection.Get<GenericData> (viewModelGuid, SELECTED_TAG);
			SelectItem (defaultSelectedData);
			RaiseAllPropertiesChanged ();
		}

		private void SelectItem(GenericData defaultSelectedData)
		{
			if (Datas != null) {
				foreach (var data in Datas) {
					if (defaultSelectedData != null && defaultSelectedData == data)
						data.IsSelected = true;
					else
						data.IsSelected = false;
				}
			}
		}

		public static async Task<ListViewModelResult> ShowGenericList(string title, List<GenericData> datas, GenericData currentTag, BaseViewModel parent = null)
		{
			string viewModelGuid = Guid.NewGuid ().ToString();
			ViewModelDataCollection.Push (viewModelGuid, ListViewModel.TITLE, title);
			ViewModelDataCollection.Push (viewModelGuid, ListViewModel.SELECTED_TAG, currentTag);
			ViewModelDataCollection.Push (viewModelGuid, ListViewModel.DATAS, datas);

			var result = new ListViewModelResult ();
			result.ViewModelValidated = await ShowModalViewModel<ListViewModel> (viewModelGuid, false, parent);
			result.SelectedTag = ViewModelDataCollection.Get<object> (viewModelGuid, OUT_SELECTED_TAG_ITEM);

			//Very important clear datas
			ViewModelDataCollection.Clear (viewModelGuid);

			return result;
		}

		public ICommand ValidateCommand
		{
			get
			{
				return new MvxCommand (() => {
					if(ValidateViewModel())
					{
						ViewModelDataCollection.Push<object> (ViewModelGuid, OUT_SELECTED_TAG_ITEM, SelectedItem.Tag);
						CloseViewModel();
					}
				});
			}
		}

		private bool ValidateViewModel()
		{
			return SelectedItem != null;
		}
	}
}


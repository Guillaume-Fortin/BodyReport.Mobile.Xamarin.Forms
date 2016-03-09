using System;
using MvvmCross.Plugins.Messenger;
using System.Windows.Input;
using MvvmCross.Core.ViewModels;
using System.Threading.Tasks;
using BodyReportMobile.Core.Message;
using System.Collections.Generic;
using Message;

namespace BodyReportMobile.Core.ViewModels
{
	public class MainViewModel  : BaseViewModel
	{
		public string ChangeLanguageLabel { get; set;}

		private string _languageFlagImageSource;

		public string LanguageFlagImageSource
		{
			get { return _languageFlagImageSource; }
			set 
			{
				if (value != _languageFlagImageSource) {
					_languageFlagImageSource = value;

					RaisePropertyChanged (() => LanguageFlagImageSource);
				}
			}
		}


		public MainViewModel (IMvxMessenger messenger) : base(messenger)
		{
		}

		public override void Init(string viewModelGuid, bool autoClearViewModelDataCollection)
		{
			base.Init (viewModelGuid, autoClearViewModelDataCollection);

			SynchronizeData ();
		}

		private void SynchronizeData()
		{
			ChangeLanguageLabel = Translation.Get (TRS.COPY);
			LanguageFlagImageSource = GeLanguageFlagImageSource (Translation.CurrentLang);

			RaiseAllPropertiesChanged ();
		}

		private string GeLanguageFlagImageSource(LangType langType)
		{
			return string.Format ("flag-{0}.png", Translation.GetLangExt (langType));
		}

		public ICommand GoToTrainingJournalCommand
		{
			get
			{
				return new MvxAsyncCommand (async () => {
					await ShowModalViewModel<TrainingJournalViewModel>(this);
				}, null, true);
			}
		}

		public ICommand GoToChangeLanguageCommand
		{
			get
			{
				return new MvxAsyncCommand (async () => {

					var datas = new List<GenericData> ();

					string trName;
					var languageValues = Enum.GetValues(typeof(LangType));
					GenericData data, currentData = null;
					foreach(LangType languageValue in languageValues)
					{
						trName = languageValue == LangType.en_US ? "English" : "Français";
						data = new GenericData(){ Tag = languageValue, Name = trName, Image = GeLanguageFlagImageSource(languageValue)};
						datas.Add(data);

						if(languageValue == Translation.CurrentLang)
							currentData = data;
					}

					var result = await ListViewModel.ShowGenericList ("Select Language", datas, currentData, this);

					if(result.ViewModelValidated && result.SelectedTag != null)
					{
						Translation.ChangeLang((LangType)result.SelectedTag);
						SynchronizeData();
					}

				}, null, true);
			}
		}
	}
}



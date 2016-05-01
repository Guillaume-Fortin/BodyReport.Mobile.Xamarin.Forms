using BodyReportMobile.Core.Framework;
using BodyReportMobile.Core.Message;
using BodyReportMobile.Core.ViewModels.Generic;
using Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace BodyReportMobile.Core.ViewModels
{
    public static class LanguageViewModel
    {
        public static string GeLanguageFlagImageSource(LangType langType)
        {
            return string.Format("flag_{0}.png", Translation.GetLangExt(langType)).Replace('-', '_');
        }

        /// <summary>
        /// Change language with user choice list view
        /// </summary>
		public static async Task<bool> DisplayChooseLanguage(BaseViewModel parentViewModel)
        {
            bool result = false;
            try
            {
                var datas = new List<GenericData>();
                string trName;
                var languageValues = Enum.GetValues(typeof(LangType));
                GenericData data, currentData = null;
                foreach (LangType languageValue in languageValues)
                {
                    trName = languageValue == LangType.en_US ? "English" : "Français";
                    data = new GenericData() { Tag = languageValue, Name = trName, Image = GeLanguageFlagImageSource(languageValue) };
                    datas.Add(data);

                    if (languageValue == Translation.CurrentLang)
                        currentData = data;
                }

                var resultShow = await ListViewModel.ShowGenericList(Translation.Get(TRS.LANGUAGE), datas, currentData, parentViewModel);

                if (resultShow.Validated && resultShow.SelectedData != null && resultShow.SelectedData.Tag != null)
                {
                    Translation.ChangeLang((LangType)resultShow.SelectedData.Tag);
                    result = true;
                }
            }
            catch
            {
            }

            return await Task.FromResult(result);
        }
    }
}

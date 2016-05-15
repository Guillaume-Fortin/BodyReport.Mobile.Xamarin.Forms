using BodyReportMobile.Core.Framework;
using BodyReportMobile.Core.Message;
using BodyReportMobile.Core.ViewModels.Generic;
using Message;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using XLabs.Ioc;

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
		public static async Task<bool> DisplayChooseLanguageAsync(BaseViewModel parentViewModel)
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

                var resultShow = await ListViewModel.ShowGenericListAsync(Translation.Get(TRS.LANGUAGE), datas, currentData, parentViewModel);

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

        public static void SaveApplicationLanguage()
        {
            try
            {
                IFileManager fileManager = Resolver.Resolve<IFileManager>();
                string temFile = Path.Combine(fileManager.GetDocumentPath(), "culture.tem");
                if (fileManager.FileExist(temFile))
                    fileManager.DeleteFile(temFile);
                fileManager.WriteAllTextFile(temFile, Translation.CurrentLang.ToString(), Encoding.UTF8);
            }
            catch(Exception except)
            {
                ILogger.Instance.Error("Unable to save application language", except);
            }
        }

        public static void ReloadApplicationLanguage()
        {
            try
            {
                LangType applicationLangType;
                //sytem culture
                if (System.Globalization.RegionInfo.CurrentRegion.TwoLetterISORegionName.ToUpper() == "FR")
                    applicationLangType = LangType.fr_FR;
                else
                    applicationLangType = LangType.en_US;

                //user culture
                IFileManager fileManager = Resolver.Resolve<IFileManager>();
                string temFile = Path.Combine(fileManager.GetDocumentPath(), "culture.tem");
                if (fileManager.FileExist(temFile))
                {
                    string langTypeStr = fileManager.ReadAllTextFile(temFile, Encoding.UTF8);
                    if (langTypeStr == LangType.fr_FR.ToString())
                        applicationLangType = LangType.fr_FR;
                }
                Translation.ChangeLang(applicationLangType);
            }
            catch (Exception except)
            {
                ILogger.Instance.Error("Unable to reload application language", except);
            }
        }
    }
}

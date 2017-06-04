using BodyReport.Message;
using BodyReportMobile.Core.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BodyReportMobile.Core.ViewModels
{
    public class ConfidentialityRulesViewModel : BaseViewModel
    {
        public ConfidentialityRulesViewModel() : base()
        {
        }

        protected override void InitTranslation()
        {
            base.InitTranslation();

            TitleLabel = Translation.Get(TRS.CONFIDENTIALITY_RULES);
            
            if (Translation.CurrentLang == LangType.fr_FR)
            {
                Rules1Label = "Les comptes utilisateurs ne sont pas communiqués à un service tiers.";
                Rules2Label = "Les mots de passe utilisateurs sont cryptés.";
                Rules3Label = "La caméra est seulement utilisée pour compléter votre profil.";
            }
            else
            {
                Rules1Label = "User accounts are not communicated to a third party service.";
                Rules2Label = "User passwords are encrypted.";
                Rules3Label = "The camera is only used to set up your profile.";
            }
        }

        public static async Task<bool> ShowAsync(BaseViewModel parent = null)
        {
            var viewModel = new ConfidentialityRulesViewModel();
            return await ShowModalViewModelAsync(viewModel, parent, false);
        }

        private string _rules1Label;
        public string Rules1Label
        {
            get { return _rules1Label; }
            set
            {
                _rules1Label = value;
                OnPropertyChanged();
            }
        }

        private string _rules2Label;
        public string Rules2Label
        {
            get { return _rules2Label; }
            set
            {
                _rules2Label = value;
                OnPropertyChanged();
            }
        }

        private string _rules3Label;
        public string Rules3Label
        {
            get { return _rules3Label; }
            set
            {
                _rules3Label = value;
                OnPropertyChanged();
            }
        }
    }
}

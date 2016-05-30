using Acr.UserDialogs;
using BodyReportMobile.Core.Framework;
using BodyReportMobile.Core.WebServices;
using BodyReport.Message;
using BodyReport.Message.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using XLabs.Ioc;

namespace BodyReportMobile.Core.ViewModels
{
    public class RegisterAccountViewModel : BaseViewModel
    {
        private string _userNameLabel;
        private string _emailLabel;
        private string _passwordLabel;
        private string _confirmPasswordLabel;
        private string _registerLabel;

        private string _userName = string.Empty;
        private string _email = string.Empty;
        private string _password = string.Empty;
        private string _confirmpassword = string.Empty;

        private IUserDialogs _userDialog;

        public RegisterAccountViewModel() : base()
        {
            _userDialog = Resolver.Resolve<IUserDialogs>();
        }

        public static async Task<bool> DisplayViewModelAsync(BaseViewModel parent = null)
        {
            var viewModel = new RegisterAccountViewModel();
            return await ShowModalViewModelAsync(viewModel, parent);
        }

        protected override void InitTranslation()
        {
            base.InitTranslation();

            TitleLabel = Translation.Get(TRS.REGISTER);
            UserNameLabel = Translation.Get(TRS.USER_NAME);
            EmailLabel = Translation.Get(TRS.EMAIL);
            PasswordLabel = Translation.Get(TRS.PASSWORD);
            ConfirmPasswordLabel = Translation.Get(TRS.CONFIRM_PASSWORD);
            RegisterLabel = Translation.Get(TRS.REGISTER);
        }

        private async Task RegisterAccountActionAsync()
        {
            if (await ValidateFieldsAsync() && await RegisterOnlineAccountAsync())
            {
                CloseViewModel();
            }
        }

        private async Task<bool> ValidateFieldsAsync()
        {
            bool result = false;

            string requiredField = null;
            if (string.IsNullOrWhiteSpace(_userName))
                requiredField = Translation.Get(TRS.USER_NAME);
            else if (string.IsNullOrWhiteSpace(_email))
                requiredField = Translation.Get(TRS.EMAIL);
            else if (string.IsNullOrWhiteSpace(_password))
                requiredField = Translation.Get(TRS.PASSWORD);
            else if (string.IsNullOrWhiteSpace(_confirmpassword))
                requiredField = Translation.Get(TRS.CONFIRM_PASSWORD);

            if (!string.IsNullOrWhiteSpace(requiredField))
            {
                string message = string.Format(Translation.Get(TRS.THE_P0_FIELD_IS_REQUIRED), requiredField);
                await _userDialog.AlertAsync(message, Translation.Get(TRS.ERROR), Translation.Get(TRS.OK));
            }
            else if (_password != _confirmpassword)
            {
                await _userDialog.AlertAsync(Translation.Get(TRS.THE_PASSWORD_AND_CONFIRMATION_PASSWORD_DO_NOT_MATCH), Translation.Get(TRS.ERROR), Translation.Get(TRS.OK));
            }
            else
                result = true;

            return await Task.FromResult(result);
        }

        private async Task<bool> RegisterOnlineAccountAsync()
        {
            bool result = false;

            try
            {
                if (await AccountWebService.RegisterAccountAsync(_userName, _email, _password))
                    result = true;
            }
            catch (WebApiException except)
            {
                await _userDialog.AlertAsync(except.Message, Translation.Get(TRS.ERROR), Translation.Get(TRS.OK));
            }
            catch (Exception except)
            {
                await _userDialog.AlertAsync(except.Message, Translation.Get(TRS.ERROR), Translation.Get(TRS.OK));
            }

            return await Task.FromResult(result);
        }
        
        #region label properties binding

        public string UserNameLabel
        {
            get
            {
                return _userNameLabel;
            }
            set
            {
                _userNameLabel = value;
                OnPropertyChanged();
            }
        }

        public string EmailLabel
        {
            get
            {
                return _emailLabel;
            }
            set
            {
                _emailLabel = value;
                OnPropertyChanged();
            }
        }

        public string PasswordLabel
        {
            get
            {
                return _passwordLabel;
            }
            set
            {
                _passwordLabel = value;
                OnPropertyChanged();
            }
        }

        public string ConfirmPasswordLabel
        {
            get
            {
                return _confirmPasswordLabel;
            }
            set
            {
                _confirmPasswordLabel = value;
                OnPropertyChanged();
            }
        }

        public string RegisterLabel
        {
            get
            {
                return _registerLabel;
            }
            set
            {
                _registerLabel = value;
                OnPropertyChanged();
            }
        }
        
        public string UserName
        {
            get
            {
                return _userName;
            }
            set
            {
                _userName = value;
                OnPropertyChanged();
            }
        }

        public string Email
        {
            get
            {
                return _email;
            }
            set
            {
                _email = value;
                OnPropertyChanged();
            }
        }

        public string Password
        {
            get
            {
                return _password;
            }
            set
            {
                _password = value;
                OnPropertyChanged();
            }
        }

        public string ConfirmPassword
        {
            get
            {
                return _confirmpassword;
            }
            set
            {
                _confirmpassword = value;
                OnPropertyChanged();
            }
        }

        public int UserNameMaxLength { get; set; } = FieldsLength.UserName.Max;
        public int PasswordMaxLength { get; set; } = FieldsLength.Password.Max;
        public int EmailMaxLength { get; set; } = FieldsLength.Email.Max;

        #endregion

        #region Command

        private ICommand _registerAccountCommand = null;
        public ICommand RegisterAccountCommand
        {
            get
            {
                if (_registerAccountCommand == null)
                {
                    _registerAccountCommand = new ViewModelCommandAsync(this, async () =>
                    {
                        await RegisterAccountActionAsync();
                    });
                }

                return _registerAccountCommand;
            }
        }

        #endregion
    }
}

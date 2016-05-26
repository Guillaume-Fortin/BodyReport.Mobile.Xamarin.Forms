using Acr.UserDialogs;
using BodyReportMobile.Core.Framework;
using BodyReportMobile.Core.Message;
using BodyReportMobile.Core.Message.Binding;
using BodyReportMobile.Core.ServiceManagers;
using BodyReportMobile.Core.ViewModels.Generic;
using BodyReportMobile.Core.WebServices;
using Message;
using SQLite.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using XLabs.Ioc;

namespace BodyReportMobile.Core.ViewModels
{
    public class EditUserProfileViewModel : BaseViewModel
    {
        private readonly SQLiteConnection _dbContext;
        private readonly UserInfoManager _userInfoManager;
        
        private string _userId;
        private UserInfo _userInfo;
        public BindingUserInfo BindingUserInfo { get; set; } = new BindingUserInfo();
        private List<Country> _countries = null;

        public EditUserProfileViewModel() : base()
        {
            _dbContext = Resolver.Resolve<ISQLite>().GetConnection();
            _userInfoManager = new UserInfoManager(_dbContext);
        }

        protected override void InitTranslation()
        {
            base.InitTranslation();

            TitleLabel = Translation.Get(TRS.MY_PROFILE);
            ValidateLabel = Translation.Get(TRS.VALIDATE);
            EditLabel = Translation.Get(TRS.EDIT);
            NameLabel = Translation.Get(TRS.NAME);
            EmailLabel = Translation.Get(TRS.EMAIL);
            SexLabel = Translation.Get(TRS.SEX);
            UnitLabel = Translation.Get(TRS.UNIT);
            UnitDescriptionLabel = Translation.Get(TRS.UNIT_SYSTEM_INFO);
            HeightLabel = Translation.Get(TRS.HEIGHT);
            WeightLabel = Translation.Get(TRS.WEIGHT);
            ZipCodeLabel = Translation.Get(TRS.ZIP_CODE);
            CountryLabel = Translation.Get(TRS.COUNTRY);
        }

        protected override async Task ShowAsync()
        {
            await base.ShowAsync();

            ActionIsInProgress = true;
            try
            {
                await SynchronizeDataAsync();
            }
            catch(Exception except)
            {
                ILogger.Instance.Error("EditUserProfileViewModel.ShowAsync error", except);
            }
            ActionIsInProgress = false;
        }

        public static async Task<bool> ShowAsync(string userId, bool allowCancelViewModel, BaseViewModel parent = null)
        {
            var viewModel = new EditUserProfileViewModel();
            viewModel._userId = userId;
            viewModel._allowCancelViewModel = allowCancelViewModel;
            return await ShowModalViewModelAsync(viewModel, parent, false, !allowCancelViewModel);
        }

        private async Task SynchronizeDataAsync()
        {
            if (_countries == null)
            {
                var countryManager = new CountryManager(_dbContext);
                _countries = countryManager.FindCountries();
            }

            if (!string.IsNullOrWhiteSpace(_userId))
            {
                var userInfoKey = new UserInfoKey() { UserId = _userId };
                _userInfo = await UserInfoWebService.GetUserInfoAsync(userInfoKey);
                BindingUserInfo.UserInfo = _userInfo;
                if (_userInfo != null)
                {
                    var user = await UserInfoWebService.GetUserAsync(_userId);
                    if (user != null)
                    {
                        BindingUserInfo.Name = user.Name;
                        BindingUserInfo.Email = user.Email;
                    }
                }
                BindingUserInfo.Sex = _userInfo.Sex;
                BindingUserInfo.Unit = _userInfo.Unit;
                BindingUserInfo.ZipCode = _userInfo.ZipCode;
                BindingUserInfo.CoutryId = _userInfo.CountryId;
                BindingUserInfo.Height = _userInfo.Height;
                BindingUserInfo.Weight = _userInfo.Weight;

                if(_countries != null && _countries.Count > 0)
                {
                    var country = _countries.Where(c => c.Id == _userInfo.CountryId).FirstOrDefault();
                    if (country != null)
                        BindingUserInfo.CountryName = country.Name;
                }
            }
        }
        
        private async Task ChangeSexActionAsync()
        {
            var datas = new List<GenericData>()
            {
                new GenericData() { Tag = TSexType.MAN, Name = Translation.Get(TRS.MAN) },
                new GenericData() { Tag = TSexType.WOMAN, Name = Translation.Get(TRS.WOMAN) }
            };
            
            GenericData currentData = null;
            if (BindingUserInfo.Sex == TSexType.MAN)
                currentData = datas[0];
            else
                currentData = datas[1];

            var result = await ListViewModel.ShowGenericListAsync(Translation.Get(TRS.SEX), datas, currentData, this);
            if (result.Validated && result.SelectedData != null)
            {
                BindingUserInfo.Sex = (TSexType)result.SelectedData.Tag;
            }
        }

        private async Task ChangeUnitActionAsync()
        {
            var datas = new List<Message.GenericData>()
            {
                new GenericData() { Tag = TUnitType.Imperial, Name = Translation.Get(TRS.IMPERIAL) },
                new GenericData() { Tag = TUnitType.Metric, Name = Translation.Get(TRS.METRIC) }
            };
            
            GenericData currentData = null;
            if (BindingUserInfo.Unit == TUnitType.Imperial)
                currentData = datas[0];
            else
                currentData = datas[1];

            var result = await ListViewModel.ShowGenericListAsync(Translation.Get(TRS.UNIT), datas, currentData, this);
            if (result.Validated && result.SelectedData != null)
            {
                BindingUserInfo.Unit = (TUnitType)result.SelectedData.Tag;
            }
        }

        private async Task ChangeCountryActionAsync()
        {
            if(_countries != null && _countries.Count > 0)
            {
                GenericData currentData = null;
                GenericData genericData;
                var datas = new List<GenericData>();
                foreach(var country in _countries)
                {
                    genericData = new GenericData() { Tag = country, Name = country.Name };

                    if (country.Id == BindingUserInfo.CoutryId)
                        currentData = genericData;

                    datas.Add(genericData);
                };
                
                var result = await ListViewModel.ShowGenericListAsync(Translation.Get(TRS.UNIT), datas, currentData, this);
                if (result.Validated && result.SelectedData != null)
                {
                    var selectCountry = (Country)result.SelectedData.Tag;
                    if (selectCountry != null)
                    {
                        BindingUserInfo.CoutryId = selectCountry.Id;
                        BindingUserInfo.CountryName = selectCountry.Name;
                    }
                }
            }
        }

        private async Task ValidateActionAsync()
        {
            try
            {
                if(ValidateFields() && await SaveDataAsync())
                {
                    CloseViewModel();
                }
            }
            catch (Exception except)
            {
                var userDialog = Resolver.Resolve<IUserDialogs>();
                await userDialog.AlertAsync(except.Message, Translation.Get(TRS.ERROR), Translation.Get(TRS.OK));
            }
        }

        private bool ValidateFields()
        {
            return !string.IsNullOrWhiteSpace(BindingUserInfo.ZipCode);
        }

        private async Task<bool> SaveDataAsync()
        {
            var userInfo = new UserInfo()
            {
                UserId = _userInfo.UserId,
                Sex = BindingUserInfo.Sex,
                Unit = BindingUserInfo.Unit,
                Height = BindingUserInfo.Height,
                Weight = BindingUserInfo.Weight,
                ZipCode = BindingUserInfo.ZipCode,
                CountryId = BindingUserInfo.CoutryId
            };

            userInfo = await UserInfoWebService.UpdateUserInfoAsync(userInfo);
            return userInfo != null;
        }

        #region label properties binding

        private string _validateLabel;
        public string ValidateLabel
        {
            get
            {
                return _validateLabel;
            }
            set
            {
                _validateLabel = value;
                OnPropertyChanged();
            }
        }

        private string _editLabel;
        public string EditLabel
        {
            get
            {
                return _editLabel;
            }
            set
            {
                _editLabel = value;
                OnPropertyChanged();
            }
        }

        private string _nameLabel;
        public string NameLabel
        {
            get
            {
                return _nameLabel;
            }
            set
            {
                _nameLabel = value;
                OnPropertyChanged();
            }
        }

        private string _emailLabel;
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

        private string _sexLabel;
        public string SexLabel
        {
            get
            {
                return _sexLabel;
            }
            set
            {
                _sexLabel = value;
                OnPropertyChanged();
            }
        }

        private string _unitLabel;
        public string UnitLabel
        {
            get
            {
                return _unitLabel;
            }
            set
            {
                _unitLabel = value;
                OnPropertyChanged();
            }
        }

        private string _unitDescriptionLabel;
        public string UnitDescriptionLabel
        {
            get
            {
                return _unitDescriptionLabel;
            }
            set
            {
                _unitDescriptionLabel = value;
                OnPropertyChanged();
            }
        }

        private string _heightLabel;
        public string HeightLabel
        {
            get
            {
                return _heightLabel;
            }
            set
            {
                _heightLabel = value;
                OnPropertyChanged();
            }
        }

        private string _weightLabel;
        public string WeightLabel
        {
            get
            {
                return _weightLabel;
            }
            set
            {
                _weightLabel = value;
                OnPropertyChanged();
            }
        }

        private string _zipCodeLabel;
        public string ZipCodeLabel
        {
            get
            {
                return _zipCodeLabel;
            }
            set
            {
                _zipCodeLabel = value;
                OnPropertyChanged();
            }
        }

        private string _countryLabel;
        public string CountryLabel
        {
            get
            {
                return _countryLabel;
            }
            set
            {
                _countryLabel = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Command

        private ICommand _validateCommand = null;
        public ICommand ValidateCommand
        {
            get
            {
                if (_validateCommand == null)
                {
                    _validateCommand = new ViewModelCommandAsync(this, async () =>
                    {
                        await ValidateActionAsync();
                    });
                }
                return _validateCommand;
            }
        }

        private ICommand _changeSexCommand = null;
        public ICommand ChangeSexCommand
        {
            get
            {
                if (_changeSexCommand == null)
                {
                    _changeSexCommand = new ViewModelCommandAsync(this, async () =>
                    {
                        await ChangeSexActionAsync();
                    });
                }
                return _changeSexCommand;
            }
        }

        private ICommand _changeUnitCommand = null;
        public ICommand ChangeUnitCommand
        {
            get
            {
                if (_changeUnitCommand == null)
                {
                    _changeUnitCommand = new ViewModelCommandAsync(this, async () =>
                    {
                        await ChangeUnitActionAsync();
                    });
                }
                return _changeUnitCommand;
            }
        }

        private ICommand _changeCountryCommand = null;
        public ICommand ChangeCountryCommand
        {
            get
            {
                if (_changeCountryCommand == null)
                {
                    _changeCountryCommand = new ViewModelCommandAsync(this, async () =>
                    {
                        await ChangeCountryActionAsync();
                    });
                }
                return _changeCountryCommand;
            }
        }

        #endregion
    }
}

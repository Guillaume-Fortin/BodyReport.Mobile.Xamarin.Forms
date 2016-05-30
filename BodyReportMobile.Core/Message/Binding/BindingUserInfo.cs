using BodyReportMobile.Core.Framework;
using BodyReport.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BodyReportMobile.Core.Message.Binding
{
    public class BindingUserInfo : NotifyPropertyChanged
    {
        public UserInfo UserInfo { get; set; }

        private string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }

        private string _email;
        public string Email
        {
            get { return _email; }
            set
            {
                _email = value;
                OnPropertyChanged();
            }
        }

        private TSexType _sex;
        public TSexType Sex
        {
            get { return _sex; }
            set
            {
                _sex = value;
                OnPropertyChanged();
                SexName = _sex == TSexType.MAN ? Translation.Get(TRS.MAN) : Translation.Get(TRS.WOMAN);
            }
        }

        private string _sexName;
        public string SexName
        {
            get { return _sexName; }
            set
            {
                _sexName = value;
                OnPropertyChanged();
            }
        }

        private TUnitType _unit;
        public TUnitType Unit
        {
            get { return _unit; }
            set
            {
                _unit = value;
                OnPropertyChanged();
            }
        }
        
        /// <summary>
        /// User Height
        /// </summary>
		private double _height;
        public double Height
        {
            get { return _height; }
            set
            {
                _height = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// User Weight
        /// </summary>
		private double _weight;
        public double Weight
        {
            get { return _weight; }
            set
            {
                _weight = value;
                OnPropertyChanged();
            }
        }

        public int ZipCodeMaxLength { get; set; } = FieldsLength.ZipCode.Max;
        
        private string _zipCode;
        public string ZipCode
        {
            get { return _zipCode; }
            set
            {
                _zipCode = value;
                OnPropertyChanged();
            }
        }

        private int _countryId;
        public int CoutryId
        {
            get { return _countryId; }
            set
            {
                _countryId = value;
                OnPropertyChanged();
            }
        }

        private string _countryName;
        public string CountryName
        {
            get { return _countryName; }
            set
            {
                _countryName = value;
                OnPropertyChanged();
            }
        }
    }
}

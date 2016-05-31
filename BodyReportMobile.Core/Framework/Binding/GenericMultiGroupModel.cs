using BodyReport.Message;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BodyReportMobile.Core.Framework.Binding
{
    public class GenericMultiGroupModel : NotifyPropertyChanged
    {
        private string _title;
        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                _title = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<object> _datas { get; set; } = new ObservableCollection<object>();

        public ObservableCollection<object> Datas
        {
            get
            {
                return _datas;
            }
            set
            {
                _datas = value;
                OnPropertyChanged();
            }
        }
    }
}

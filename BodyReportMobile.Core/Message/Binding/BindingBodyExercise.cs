using BodyReport.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BodyReportMobile.Core.Message.Binding
{
    public class BindingBodyExercise : NotifyPropertyChanged
    {
        public BodyExercise BodyExercise { get; set; }

        private string _image;
        public string Image
        {
            get { return _image; }
            set
            {
                _image = value;
                OnPropertyChanged();
            }
        }

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

        private bool _selected;
        public bool Selected
        {
            get { return _selected; }
            set
            {
                _selected = value;
                OnPropertyChanged();
            }
        }
    }
}

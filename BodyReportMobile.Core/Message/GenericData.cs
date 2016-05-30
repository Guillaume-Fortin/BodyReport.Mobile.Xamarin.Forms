using BodyReport.Message;
using System;

using System.ComponentModel;

namespace BodyReportMobile.Core.Message
{
	public class GenericData : NotifyPropertyChanged
    {
		private string _image = string.Empty;
		private string _name;
		private string _description;
		private bool _isSelected;
		public object Tag;

		public string Image
		{
			get { return _image; }
			set 
			{ 
				_image = value;
				OnPropertyChanged();
			}
		}

		public string Name
		{
			get { return _name; }
			set 
			{ 
				_name = value;
				OnPropertyChanged();
			}
		}

		public string Description
		{
			get { return _description; }
			set 
			{ 
				_description = value;
				OnPropertyChanged();
			}
		}

		public bool IsSelected
		{
			get { return _isSelected; }
			set 
			{ 
				_isSelected = value;
				OnPropertyChanged();
			}
		}
	}
}


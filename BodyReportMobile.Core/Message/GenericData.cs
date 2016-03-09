using System;

using System.ComponentModel;

namespace BodyReportMobile.Core.Message
{
	public class GenericData : INotifyPropertyChanged
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
				OnPropertyChanged("Image");
			}
		}

		public string Name
		{
			get { return _name; }
			set 
			{ 
				_name = value;
				OnPropertyChanged("Name");
			}
		}

		public string Description
		{
			get { return _description; }
			set 
			{ 
				_description = value;
				OnPropertyChanged("Description");
			}
		}

		public bool IsSelected
		{
			get { return _isSelected; }
			set 
			{ 
				_isSelected = value;
				OnPropertyChanged("IsSelected");
			}
		}

		#region INotifyPropertyChanged
		public event PropertyChangedEventHandler PropertyChanged;

		public void OnPropertyChanged(string propertyName)
		{
			if (this.PropertyChanged != null)
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
		#endregion
	}
}


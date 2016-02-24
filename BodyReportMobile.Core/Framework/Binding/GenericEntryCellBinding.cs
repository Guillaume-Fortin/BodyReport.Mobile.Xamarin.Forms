using System;
using System.Reflection;

namespace BodyReportMobile.Core
{
	public class GenericEntryCellBinding//<T1, T2>
	{
		public string Title { get; set; }
		public Object Object { get{ return _object; } }

		private Object _object;
		private string _objectPropertyName;
		private PropertyInfo _propertyInfo;

		public GenericEntryCellBinding(Object obj, string title, string objectPropertyName)
		{
			_object = obj;
			Title = title;
			_objectPropertyName = objectPropertyName;
			_propertyInfo = obj.GetType().GetRuntimeProperty (_objectPropertyName);
		}

		public Object Value
		{
			get
			{
				return _propertyInfo.GetValue (_object);
			}
			set
			{
				_propertyInfo.SetValue (_object, value);
			}
		}
	}
}


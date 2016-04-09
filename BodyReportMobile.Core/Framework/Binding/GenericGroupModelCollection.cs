using System;
using System.Collections.ObjectModel;

namespace BodyReportMobile.Core.Framework.Binding
{
	public class GenericGroupModelCollection<T> : ObservableCollection<T>
	{
		public string LongName { get; set; }
		public string ShortName { get; set; }
	}
}


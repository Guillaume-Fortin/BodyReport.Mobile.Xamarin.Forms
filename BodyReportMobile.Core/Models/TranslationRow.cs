using System;

namespace BodyReportMobile.Core.Models
{
	public class TranslationRow
	{
		/// <summary>
		/// Regionn Culture id
		/// </summary>
		public int CultureId
		{
			get;
			set;
		}

		/// <summary>
		/// Translation key
		/// </summary>
		public string Key
		{
			get;
			set;
		}

		/// <summary>
		/// Translation value
		/// </summary>
		public string Value
		{
			get;
			set;
		}
	}
}


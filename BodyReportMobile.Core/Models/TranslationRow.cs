using System;
using SQLite.Net.Attributes;

namespace BodyReportMobile.Core
{
	[Table("Translation")]
	public class TranslationRow
	{
		/// <summary>
		/// Regionn Culture id
		/// </summary>
		[PrimaryKey, Column("CultureId")]
		public int CultureId
		{
			get;
			set;
		}

		/// <summary>
		/// Translation key
		/// </summary>
		//[PrimaryKey, Column("Key"), MaxLength(256)] //Multiple Key not supported?? oO
		[Indexed, Column("Key"), MaxLength(256)] 
		public string Key
		{
			get;
			set;
		}

		/// <summary>
		/// Translation value
		/// </summary>
		[MaxLength(2000)]
		public string Value
		{
			get;
			set;
		}
	}
}


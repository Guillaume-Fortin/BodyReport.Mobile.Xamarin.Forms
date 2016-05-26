using System;
using SQLite.Net.Attributes;

namespace BodyReportMobile.Core.Models
{
	[Table("TrainingWeek")]
	public class TrainingWeekRow
	{
		// <summary>
		/// UserId
		/// </summary>
		[PrimaryKey, Column("UserId")]
		[MaxLength(450)]
		public string UserId { get; set; }
		/// <summary>
		/// Year
		/// </summary>
		[PrimaryKey, Column("Year")]
		public int Year { get; set; }
		/// <summary>
		/// Week of year
		/// </summary>
		[PrimaryKey, Column("WeekOfYear")]
		public int WeekOfYear { get; set; }
		/// <summary>
		/// User Height
		/// </summary>
		public double UserHeight { get; set; }
		/// <summary>
		/// User Weight
		/// </summary>
		public double UserWeight { get; set; }
		/// <summary>
		/// Unit Type
		/// </summary>
		public int Unit
		{
			get;
			set;
		}
        /// <summary>
		/// Modification Date
		/// </summary>
		public DateTime ModificationDate
        {
            get;
            set;
        }
    }
}


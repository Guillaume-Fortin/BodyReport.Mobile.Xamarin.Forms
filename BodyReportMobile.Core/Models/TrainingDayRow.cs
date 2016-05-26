using System;
using SQLite.Net.Attributes;

namespace BodyReportMobile.Core.Models
{
	[Table("TrainingDay")]
	public class TrainingDayRow
	{
		/// <summary>
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
		/// Day of week
		/// </summary>
		[PrimaryKey, Column("DayOfWeek")]
		public int DayOfWeek { get; set; }
		/// <summary>
		/// Training day id
		/// </summary>
		[PrimaryKey, Column("TrainingDayId")]
		public int TrainingDayId { get; set; }
		/// <summary>
		/// Begin hour
		/// </summary>
		public DateTime BeginHour { get; set; }
		/// <summary>
		/// End hour
		/// </summary>
		public DateTime EndHour { get; set; }
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


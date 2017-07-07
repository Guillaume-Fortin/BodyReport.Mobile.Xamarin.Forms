using System;

namespace BodyReportMobile.Core.Models
{
	public class TrainingWeekRow
	{
		// <summary>
		/// UserId
		/// </summary>
		public string UserId { get; set; }
		/// <summary>
		/// Year
		/// </summary>
		public int Year { get; set; }
		/// <summary>
		/// Week of year
		/// </summary>
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


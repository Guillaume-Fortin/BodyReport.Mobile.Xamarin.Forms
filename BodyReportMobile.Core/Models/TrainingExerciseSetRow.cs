using System;
using SQLite.Net.Attributes;

namespace BodyReportMobile.Core.Models
{
	[Table("TrainingExerciseSet")]
	public class TrainingExerciseSetRow
	{
        /// <summary>
        /// UserId
        /// </summary>
        [PrimaryKey, Column("UserId")]
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
        /// Id of training exercise
        /// </summary>
        [PrimaryKey, Column("TrainingExerciseId")]
        public int TrainingExerciseId { get; set; }
        /// <summary>
        /// Id of set/Rep
        /// </summary>
        [PrimaryKey, Column("Id")]
        public int Id { get; set; }
		/// <summary>
		/// Number of sets
		/// </summary>
		public int NumberOfSets { get; set; }
		/// <summary>
		/// Number of reps
		/// </summary>
		public int NumberOfReps { get; set; }
		/// <summary>
		/// Weight
		/// </summary>
		public double Weight { get; set; }
		/// <summary>
		/// Unit Type
		/// </summary>
		public int Unit
		{
			get;
			set;
		}
	}
}


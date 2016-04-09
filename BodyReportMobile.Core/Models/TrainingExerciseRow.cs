using System;
using SQLite.Net.Attributes;

namespace BodyReportMobile.Core.Models
{
	[Table("TrainingExercise")]
	public class TrainingExerciseRow
	{
		/// <summary>
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
		/// Day of week
		/// </summary>
		public int DayOfWeek { get; set; }
		/// <summary>
		/// Training day id
		/// </summary>
		public int TrainingDayId { get; set; }
		/// <summary>
		/// Id of training exercise
		/// </summary>
		public int Id { get; set; }
		/// <summary>
		/// Id of body exercise
		/// </summary>
		public int BodyExerciseId { get; set; }
		/// <summary>
		/// Rest time (second)
		/// </summary>
		public int RestTime { get; set; }
	}
}


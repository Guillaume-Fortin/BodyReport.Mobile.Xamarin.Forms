using System;
using SQLite.Net.Attributes;

namespace BodyReportMobile.Core.Models
{
	[Table("Muscle")]
	public class MuscleRow
	{
		/// <summary>
		/// Muscular Id
		/// </summary>
		[PrimaryKey, Column("Id")]
		public int Id { get; set; }

		/// <summary>
		/// Muscular group Id
		/// </summary>
		public int MuscularGroupId { get; set; }
	}
}


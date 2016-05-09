using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BodyReportMobile.Core.Models
{
    [Table("MuscularGroup")]
    public class MuscularGroupRow
    {
        /// <summary>
        /// Muscular group Id
        /// </summary>
        [PrimaryKey, Column("Id")]
        public int Id { get; set; }
    }
}

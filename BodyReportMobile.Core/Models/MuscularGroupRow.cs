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

        /// <summary>
        /// Need it for correct update bug:
        /// It seems that sqlite-net library (and not this project) doesn't handle well entities with no other fields apart of the primary key. To fix it, just add any property to the Journey class
        /// https://bitbucket.org/twincoders/sqlite-net-extensions/issues/91/sqlitenetsqliteexception-near-where-syntax
        /// </summary>
        public int Foo { get; set; }
    }
}

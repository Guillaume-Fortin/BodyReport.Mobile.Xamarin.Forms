using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BodyReportMobile.Core.Models
{
    public class CountryRow
    {
        /// <summary>
        /// Id (Key)
        /// </summary>
        [PrimaryKey, Column("Id")]
        public int Id
        {
            get;
            set;
        }

        /// <summary>
        /// Name
        /// </summary>
        [MaxLength(400)]
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Short name
        /// </summary>
        [MaxLength(10)]
        public string ShortName
        {
            get;
            set;
        }
    }
}

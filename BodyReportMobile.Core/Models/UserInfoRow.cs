using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SQLite.Net.Attributes;

namespace BodyReport.Models
{
	[Table("UserInfo")]
    public class UserInfoRow
    {
        /// <summary>
        /// UserId (Key)
        /// </summary>
		[PrimaryKey, Column("UserId")]
		[MaxLength(450)]
        public string UserId
        {
            get;
            set;
        }

        /// <summary>
        /// Sex
        /// </summary>
        public int Sex
        {
            get;
            set;
        }

        /// <summary>
        /// Unit system
        /// </summary>
        public int Unit
        {
            get;
            set;
        }

        /// <summary>
        /// Height
        /// </summary>
        public double Height
        {
            get;
            set;
        }

        /// <summary>
        /// Weight
        /// </summary>
        public double Weight
        {
            get;
            set;
        }

        /// <summary>
        /// PostalCode
        /// </summary>
		[MaxLength(80)]
        public string ZipCode
        {
            get;
            set;
        }

        /// <summary>
        /// CountryId
        /// </summary>
        public int CountryId
        {
            get;
            set;
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CoE.Ideas.Core.Internal.WordPress
{
    [Table("wp_users")]
    internal class UserInternal
    {
        /// <summary>
        /// Unique identifier for the user.
        /// </summary>
        [Column("ID")]
        public int Id { get; set; }

        /// <summary>
        /// Login name for the user.
        /// </summary>
        [Column("user_login")]
        public string UserName { get; set; }

        /// <summary>
        /// Password Hash 
        /// </summary>
        [Column("user_pass")]
        public string Password { get; set; }

        /// <summary>
        /// Display name for the user.
        /// </summary>
        [Column("display_name")]
        public string Name { get; set; }

        /// <summary>
        /// The email address for the user.
        /// </summary>
        [Column("user_email")]
        public string Email { get; set; }

        /// <summary>
        /// URL of the user.
        /// </summary>
        [Column("user_url")]
        public string Url { get; set; }

        [InverseProperty("User")]
        public virtual ICollection<UserMetadataInternal> Metadata { get; set; }
    }

}

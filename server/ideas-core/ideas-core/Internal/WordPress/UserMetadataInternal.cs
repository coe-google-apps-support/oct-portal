using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CoE.Ideas.Core.Internal.WordPress
{
    [Table("wp_usermeta")]
    internal class UserMetadataInternal
    {
        [Column("umeta_id")]
        public int Id { get; set; }

        [Column("user_id")]
        public int UserId { get; set; }

        [Column("meta_key")]
        public string Key { get; set; }

        [Column("meta_value")]
        public string Value { get; set; }

        [ForeignKey("UserId")]
        public UserInternal User { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CoE.Ideas.Core.Internal.WordPress
{
    [Table("wp_options")]
    internal class OptionInternal
    {
        [Column("option_id")]
        public int Id { get; set; }

        [Column("option_name")]
        public string Name { get; set; }

        [Column("option_value")]
        public string Value { get; set; }
    }
}

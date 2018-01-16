using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CoE.Ideas.Core.Internal
{
    /// <summary>
    /// User-defined categories
    /// </summary>
    [Table("Tags")]
    internal class TagInternal : EntityBaseInternal
    {
        /// <summary>
        /// Name of the tag
        /// </summary>
        [Required]
        public string Name { get; set; }


        /// <summary>
        /// The date and time the issue was created
        /// </summary>
        public DateTimeOffset CreatedDate { get; set; }
    }
}

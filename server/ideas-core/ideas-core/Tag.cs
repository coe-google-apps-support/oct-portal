using System;
using System.ComponentModel.DataAnnotations;

namespace CoE.Ideas.Core
{
    /// <summary>
    /// User-defined categories
    /// </summary>
    public class Tag : EntityBase
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
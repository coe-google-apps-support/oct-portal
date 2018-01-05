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
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CoE.Issues.Server.Models
{
    public class IssueInfo
    {
        public int Id { get; set; }

        /// <summary>
        /// The short title of the idea
        /// </summary>
        [Required]
        [MaxLength(255)]
        public string Title { get; set; }

        /// <summary>
        /// The long description of the idea, can be HTML formatted
        /// </summary>
        [Required]
        public string Description { get; set; }

        [Required]
        public DateTimeOffset CreatedDate { get; set; }

        public string Url { get; set; }
    }
}

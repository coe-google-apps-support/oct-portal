using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CoE.Ideas.Core
{
    /// <summary>
    /// An idea is a thought, request, or some other interaction with the City of Edmonton
    /// </summary>
    public class Idea : EntityBase
    {
        /// <summary>
        /// The unique identifier of an idea (blog post) in WordPress
        /// </summary>
        [Required]
        public int WordPressKey { get; set; }

        /// <summary>
        /// The short title of the idea
        /// </summary>
        [Required]
        public string Title { get; set; }

        /// <summary>
        /// The long description of the idea, can be HTML formatted
        /// </summary>
        [Required]
        public string Description { get; set; }

        // best practice is to have only one-way navigation properties, where possible
        /// <summary>
        /// The people that have some stake in the idea, will always include the owner
        /// </summary>
        public ICollection<Stakeholder> Stakeholders { get; set; }

        /// <summary>
        /// User-defined tags to categorize the idea
        /// </summary>
        public ICollection<Tag> Tags { get; set; }

    }
}

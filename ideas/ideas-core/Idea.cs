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

        /// <summary>
        /// The address of the idea for display in a browser
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// The date and time the issue was created
        /// </summary>
        public DateTimeOffset CreatedDate { get; set; }

        /// <summary>
        /// Does the idea have a business sponsor? 
        /// </summary>
        public bool HasBusinessSponsor { get; set; }

        /// <summary>
        /// The name of the Business Sponsor
        /// </summary>
        public string BusinessSponsor { get; set; }

        /// <summary>
        /// Do you have a budget for this initiative? 
        /// </summary>
        public bool HasBudget { get; set; }

        /// <summary>
        /// What is your expected target date?
        /// </summary>
        public DateTime? ExpectedTargetDate { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CoE.Ideas.Core.Internal.Initiatives
{
    /// <summary>
    /// An idea is a thought, request, or some other interaction with the City of Edmonton
    /// </summary>
    [Table("Ideas")]
    internal class IdeaInternal : EntityBaseInternal
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
        public virtual ICollection<StakeholderInternal> Stakeholders { get; set; }

        /// <summary>
        /// The personal currently assigned to the initiative, usually a Business Analyst
        /// </summary>
        /// <remarks>
        /// Can be null
        /// </remarks>
        public virtual PersonInternal Assignee { get; set; }

        /// <summary>
        /// User-defined tags to categorize the idea
        /// </summary>
        public virtual ICollection<TagInternal> Tags { get; set; }

        /// <summary>
        /// The address of the idea for display in a browser
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// The date and time the issue was created
        /// </summary>
        public DateTimeOffset CreatedDate { get; set; }

        /// <summary>
        /// Unique identifier in Work Item Tracking system (Remedy)
        /// </summary>
        public string WorkItemId { get; set; }

        /// <summary>
        /// Status of the Initiative
        /// </summary>
        public InitiativeStatusInternal Status { get; set; }

        /// <summary>
        /// The department where the initiative is a part of. Can be different
        /// than the department the owner is in.
        /// </summary>
        [Display(Name = "Department", Description = "The department for this initiative")]
        public DepartmentInternal Department { get; set; }

        /// <summary>
        /// Does the idea have a business sponsor? 
        /// </summary>
        [Display(Name = "Has Business Sponsor", Description = "Do you have a business sponsor?")]
        public bool HasBusinessSponsor { get; set; }

        /// <summary>
        /// The name of the Business Sponsor
        /// </summary>
        [Display(Name = "Sponsor Name", Description = "What is the Business Sponsor's name?")]
        public string BusinessSponsorName { get; set; }

        /// <summary>
        /// The email address of the Business Sponsor
        /// </summary>
        [Display(Name = "Sponsor Email", Description = "What is the Business Sponsor's email address?")]
        public string BusinessSponsorEmail { get; set; }


        /// <summary>
        /// Do you have a budget for this initiative? 
        /// </summary>
        [Display(Name = "Budget", Description = "Do you have a budget for this initiative? ")]
        public bool HasBudget { get; set; }

        /// <summary>
        /// What is your expected target date?
        /// </summary>
        [Display(Name = "Expected Target Date", Description = "What is your expected target date?")]
        public DateTime? ExpectedTargetDate { get; set; }

        /// <summary>
        /// Describe the business benefits of the initiative.
        /// </summary>
        [Display(Name = "Benefits", Description = "Describe the business benefits of the initiative.")]
        public string BusinessBenefites { get; set; }

        [Display(Name = "Alignment", Description = "Describe how this initiative aligns with and supports the One City objective.")]
        public string OneCityAlignment { get; set; }

        /// <summary>
        /// Business case for the initiative
        /// </summary>
        [Display(Name = "Business Case URL", Description = "The location of the businses case for the initiative")]
        public string BusinessCaseUrl { get; set; }

    }
}

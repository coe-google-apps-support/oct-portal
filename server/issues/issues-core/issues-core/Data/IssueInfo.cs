using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CoE.Issues.Core.Data
{
    public class IssueInfo
    {
        public int Id { get; private set; }

        /// <summary>
        /// The short title of the idea
        /// </summary>
        [Required]
        [MinLength(3)]
        [MaxLength(255)]
        public string Title { get; private set; }

        /// <summary>
        /// The long description of the idea, can be HTML formatted
        /// </summary>
        [Required]
        [MinLength(3)]
        public string Description { get; private set; }

        [Required]
        public DateTime CreatedDate { get; private set; }

        public string AssigneeEmail { get; private set; }
        public string AssigneeGroup { get; private set; }
        public string RequestorName { get; private set; }
        public string RemedyStatus { get; private set; }
        public string ReferenceId { get; private set; }

        internal static IssueInfo Create(Issue issue)
        {
            return new IssueInfo()
            {
                Id = issue.Id,
                Title = issue.Title,
                Description = issue.Description,
                CreatedDate = issue.CreatedDate,
                AssigneeEmail = issue.AssigneeEmail,
                AssigneeGroup = issue.AssigneeGroup,
                RequestorName = issue.RequestorName,
                RemedyStatus = issue.RemedyStatus,
                ReferenceId = issue.ReferenceId


            };
        }

    }
}

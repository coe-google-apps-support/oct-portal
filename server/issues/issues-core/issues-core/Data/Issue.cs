using CoE.Ideas.Shared.Data;
using EnsureThat;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CoE.Issues.Core.Data
{
    public class Issue : AggregateRoot<int>
    {
        public Issue(Guid uid) : this()
        {
            Uid = uid;
        }

        private Issue() : base() // required for EF
        {
        }

        public static Issue Create(
            string title, 
            string description,
            int ownerPersonId = -1,
            string referenceId = ""
            )
        {
            Ensure.String.IsNotNullOrWhiteSpace(title, nameof(title));
            Ensure.String.IsNotNullOrWhiteSpace(description, nameof(description));

            var issue = new Issue(Guid.NewGuid())
            {
                Title = title,
                Description = description,
                Stakeholders = new List<Stakeholder>()
                {
                    Stakeholder.Create(ownerPersonId, StakeholderType.Requestor)
                },
                CreatedDate = DateTime.UtcNow
            };
            
            return issue;
        }


        public Guid Uid { get; private set; }

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

        /// <summary>
        /// The date this issue was created.
        /// </summary>
        public DateTimeOffset CreatedDate { get; private set; }

        /// <summary>
        /// The people that have some stake in the idea, will always include the owner
        /// </summary>
        public ICollection<Stakeholder> Stakeholders { get; private set; }

        /// <summary>
        /// The reference id from another system (currently Remedy).
        /// </summary>
        public string ReferenceId { get; private set; }

        /// <summary>
        /// The person currently assigned to the issue.
        /// </summary>
        /// <remarks>
        /// Can be null
        /// </remarks>
        public int? AssigneeId { get; private set; }

        public string IncidentId { get; private set; }


    }
}

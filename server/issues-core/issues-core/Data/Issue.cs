using CoE.Ideas.Shared.Data;
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

        public static Issue CreateIssue(string title, string description)
        {
            return new Issue()
            {
                Title = title,
                Description = description
            };
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

        public DateTimeOffset CreatedDate { get; private set; }

        /// <summary>
        /// The person currently assigned to the initiative, usually a Business Analyst
        /// </summary>
        /// <remarks>
        /// Can be null
        /// </remarks>
        public int? AssigneeId { get; private set; }

    }
}

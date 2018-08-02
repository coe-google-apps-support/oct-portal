﻿using CoE.Ideas.Shared.Data;
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

        public Issue() : base() // required for EF
        {

        }
        public static Issue Create(
            string title, 
            string description,
            string referenceId,
            string remedystatus,
            string requestorname,
            string assigneeemail,
            string assigneegroup,
            DateTime createddate,
            string urgency,
            int ownerPersonId
            )
        {
            Ensure.String.IsNotNullOrWhiteSpace(title, nameof(title));


            var issue = new Issue(Guid.NewGuid())
            {
                Title = title,
                Description = description,
                ReferenceId = referenceId,
                RemedyStatus = remedystatus,
                RequestorName = requestorname,
                AssigneeEmail = assigneeemail,
                AssigneeGroup = assigneegroup,
                Urgency = urgency,

                Stakeholders = new List<Stakeholder>()
                {
                    Stakeholder.Create(ownerPersonId, StakeholderType.Requestor)
                },
                CreatedDate = createddate
            };

            

            return issue;
        }


        public static Issue Create(
            int id,
            string title,
            string description,
            string referenceId,
            string remedystatus,
            string requestorname,
            string assigneeemail,
            string assigneegroup,
            DateTime createddate,
            string urgency,
            int ownerPersonId
            )
        {
            Ensure.String.IsNotNullOrWhiteSpace(title, nameof(title));


            var issue = new Issue(Guid.NewGuid())
            {
                Id = id,
                Title = title,
                Description = description,
                ReferenceId = referenceId,
                RemedyStatus = remedystatus,
                RequestorName = requestorname,
                AssigneeEmail = assigneeemail,
                AssigneeGroup = assigneegroup,
                Urgency = urgency,

                Stakeholders = new List<Stakeholder>()
                {
                    Stakeholder.Create(ownerPersonId, StakeholderType.Requestor)
                },
                CreatedDate = createddate
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

        public int Id { get; private set; }

        /// <summary>
        /// The long description of the idea, can be HTML formatted
        /// </summary>
        public string Description { get; private set; }

        public DateTime CreatedDate { get;  set; }

        public ICollection<Stakeholder> Stakeholders { get; private set; }

        public string AssigneeEmail { get; set; }
        public string AssigneeGroup { get; set; }

        public string RequestorName { get; set; }
        public string RemedyStatus { get; set; }
        public string ReferenceId { get; set; }
        public string Urgency { get; set; }

    }
}

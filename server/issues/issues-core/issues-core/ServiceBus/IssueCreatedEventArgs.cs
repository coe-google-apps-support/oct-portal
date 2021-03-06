﻿using CoE.Ideas.Shared.People;
using CoE.Issues.Core.Data;
using System;

namespace CoE.Issues.Core.ServiceBus
{
    public class IssueCreatedEventArgs
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string RemedyStatus { get; set; }
        public string RequestorGivenName { get; set; }
        public string RequestorSurnName { get; set; }
        public string RequestorTelephone { get; set; }
        public string RequestorEmail { get; set; }
        public string ReferenceId { get; set; }
        public string RequestorDisplayName { get; set; }
        public string AssigneeEmail { get; set; }
        public string AssigneeGroup { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Urgency { get; set; }



    }
}

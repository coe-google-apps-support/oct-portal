using CoE.Issues.Core.Data;
using System;

namespace CoE.Issues.Core.ServiceBus
{
    public class IssueCreatedEventArgs
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string RemedyStatus { get; set; }
        public string RequestorName { get; set; }
        public string ReferenceId { get; set; }

        public string AssigneeEmail { get; set; }
        public DateTime CreatedDate { get; set; }



    }
}

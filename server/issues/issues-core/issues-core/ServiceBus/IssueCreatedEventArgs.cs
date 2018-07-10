using CoE.Issues.Core.Data;

namespace CoE.Issues.Core.ServiceBus
{
    public class IssueCreatedEventArgs
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string AssigneeEmail { get; set; }
        public string RequestorEmail { get; set; }
        public string RemedyStatus { get; set; }
        public string ReferenceId { get; set; }
    }
}

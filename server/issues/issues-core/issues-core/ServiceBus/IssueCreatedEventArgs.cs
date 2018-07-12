using CoE.Ideas.Shared.People;
using CoE.Issues.Core.Data;
using CoE.Issues.Core.Remedy;
using System;

namespace CoE.Issues.Core.ServiceBus
{
    public class IssueCreatedEventArgs
    {
        public Incident Incident { get; set; }

        public PersonData Assignee { get; set; }
        public PersonData Submitter { get; set; }
        public PersonData Customer { get; set; }
    }
}

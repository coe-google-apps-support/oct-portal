using System;
using System.Collections.Generic;
using System.Text;

namespace CoE.Issues.Core.ServiceBus
{
    public class IssueNewCreatedEventArgs : IncidentUpdatedEventArgs
    {
        public string IncidentId { get; set; }
    }
}

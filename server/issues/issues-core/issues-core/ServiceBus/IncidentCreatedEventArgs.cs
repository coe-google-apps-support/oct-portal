using System;
using System.Collections.Generic;
using System.Text;

namespace CoE.Issues.Core.ServiceBus
{
    public class IncidentCreatedEventArgs: IssueCreatedEventArgs
    {
        public string IncidentId { get; set; }
        public DateTime? EtaUtc { get; set; }
    }
}

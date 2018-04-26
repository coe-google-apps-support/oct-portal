using System;
using System.Collections.Generic;
using System.Text;

namespace CoE.Ideas.Core.ServiceBus
{
    public class WorkOrderUpdatedEventArgs
    {
        public string WorkOrderId { get; set; }
        public DateTime UpdatedDateUtc { get; set; }
        public string UpdatedStatus { get; set; }
        public string AssigneeEmail { get; set; }
        public string AssigneeDisplayName { get; set; }
        public DateTime? EtaUtc { get; set; }
        public string RemedyStatus { get; set; }
    }
}

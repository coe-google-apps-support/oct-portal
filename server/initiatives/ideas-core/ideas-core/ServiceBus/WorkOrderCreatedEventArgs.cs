using System;
using System.Collections.Generic;
using System.Text;

namespace CoE.Ideas.Core.ServiceBus
{
    public class WorkOrderCreatedEventArgs : InitiativeCreatedEventArgs
    {
        public string WorkOrderId { get; set; }
        public DateTime? EtaUtc { get; set; }
    }
}

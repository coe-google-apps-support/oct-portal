using System;
using System.Collections.Generic;
using System.Text;

namespace CoE.Ideas.Core.ServiceBus
{
    public class WorkOderCreatedEventArgs : InitiativeCreatedEventArgs
    {
        public string WorkOrderId { get; set; }
    }
}

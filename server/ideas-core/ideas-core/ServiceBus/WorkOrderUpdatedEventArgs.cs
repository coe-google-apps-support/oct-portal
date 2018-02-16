using System;
using System.Collections.Generic;
using System.Text;

namespace CoE.Ideas.Core.ServiceBus
{
    public class WorkOrderUpdatedEventArgs
    {
        public string UpdatedStatus { get; set; }
        public DateTime UpdatedDateUtc { get; set; }
        public string WorkOrderId { get; set; }
    }
}

using CoE.Ideas.Core.WordPress;
using Microsoft.Azure.ServiceBus;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CoE.Ideas.Core.ServiceBus
{
    public interface IInitiativeMessageReceiver
    {
        void ReceiveMessages(Func<InitiativeCreatedEventArgs, CancellationToken, Task> initiativeCreatedHandler = null,
            Func<WorkOrderCreatedEventArgs, CancellationToken, Task> workOrderCreatedHandler = null,
            Func<WorkOrderUpdatedEventArgs, CancellationToken, Task> workOrderUpdatedHandler = null,
            Func<InitiativeLoggedEventArgs, CancellationToken, Task> initiativeLoggedHandler = null,
            MessageHandlerOptions options = null);
    }

}

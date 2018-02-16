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
        void ReceiveInitiativeCreated(Func<InitiativeCreatedEventArgs, CancellationToken, Task> handler, MessageHandlerOptions options);

        void ReceiveInitiativeWorkItemCreated(Func<WorkOrderCreatedEventArgs, CancellationToken, Task> handler, MessageHandlerOptions options);

        void ReceiveWorkOrderUpdated(Func<WorkOrderUpdatedEventArgs, CancellationToken, Task> handler, MessageHandlerOptions options);

        void ReceiveInitiativeLogged(Func<InitiativeLoggedEventArgs, CancellationToken, Task> handler, MessageHandlerOptions options);
    }

}

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
        void ReceiveInitiativeCreated(Func<InitiativeCreatedEventArgs, CancellationToken, Task> handler,
            Func<ExceptionReceivedEventArgs, Task> exceptionReceivedHandler);

        void ReceiveInitiativeWorkItemCreated(Func<WorkOderCreatedEventArgs, CancellationToken, Task> handler, MessageHandlerOptions options);
        void ReceiveInitiativeWorkItemCreated(Func<WorkOderCreatedEventArgs, CancellationToken, Task> handler, 
            Func<ExceptionReceivedEventArgs, Task> exceptionReceivedHandler);
    }
}

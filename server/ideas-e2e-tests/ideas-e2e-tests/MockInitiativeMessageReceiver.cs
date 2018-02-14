using CoE.Ideas.Core;
using CoE.Ideas.Core.ServiceBus;
using Microsoft.Azure.ServiceBus;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CoE.Ideas.EndToEnd.Tests
{
    internal class MockInitiativeMessageReceiver : IInitiativeMessageReceiver
    {
        public MockInitiativeMessageReceiver()
        {
            CreatedHandlers = new List<Func<InitiativeCreatedEventArgs, CancellationToken, Task>>();
            WorkOrderCreatedHandlers = new List<Func<WorkOderCreatedEventArgs, CancellationToken, Task>>();
        }

        internal IList<Func<InitiativeCreatedEventArgs, CancellationToken, Task>> CreatedHandlers
        {
            get;
            private set;
        }

        internal IList<Func<WorkOderCreatedEventArgs, CancellationToken, Task>> WorkOrderCreatedHandlers;



        public void ReceiveInitiativeCreated(Func<InitiativeCreatedEventArgs, CancellationToken, Task> handler, MessageHandlerOptions options)
        {
            CreatedHandlers.Add(handler);
        }

        public void ReceiveInitiativeCreated(Func<InitiativeCreatedEventArgs, CancellationToken, Task> handler, Func<ExceptionReceivedEventArgs, Task> exceptionReceivedHandler)
        {
            CreatedHandlers.Add(handler);
        }

        public void ReceiveInitiativeWorkItemCreated(Func<WorkOderCreatedEventArgs, CancellationToken, Task> handler, MessageHandlerOptions options)
        {
            WorkOrderCreatedHandlers.Add(handler);
        }

        public void ReceiveInitiativeWorkItemCreated(Func<WorkOderCreatedEventArgs, CancellationToken, Task> handler, Func<ExceptionReceivedEventArgs, Task> exceptionReceivedHandler)
        {
            WorkOrderCreatedHandlers.Add(handler);
        }
    }
}

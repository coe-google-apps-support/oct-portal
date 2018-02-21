using CoE.Ideas.Core.ServiceBus;
using Microsoft.Azure.ServiceBus;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CoE.Ideas.Core.Tests
{
    internal class MockInitiativeMessageReceiver : IInitiativeMessageReceiver
    {
        public MockInitiativeMessageReceiver()
        {
            CreatedHandlers = new List<Func<InitiativeCreatedEventArgs, CancellationToken, Task>>();
            WorkOrderCreatedHandlers = new List<Func<WorkOrderCreatedEventArgs, CancellationToken, Task>>();
            WorkOrderUpdatedHandlers = new List<Func<WorkOrderUpdatedEventArgs, CancellationToken, Task>>();
            InitiativeLoggedHandlers = new List<Func<InitiativeLoggedEventArgs, CancellationToken, Task>>();
        }

        internal IList<Func<InitiativeCreatedEventArgs, CancellationToken, Task>> CreatedHandlers { get; private set; }

        internal IList<Func<WorkOrderCreatedEventArgs, CancellationToken, Task>> WorkOrderCreatedHandlers { get; private set; }
        internal IList<Func<WorkOrderUpdatedEventArgs, CancellationToken, Task>> WorkOrderUpdatedHandlers { get; private set; }

        internal IList<Func<InitiativeLoggedEventArgs, CancellationToken, Task>> InitiativeLoggedHandlers { get; private set; }

        public Task CloseAsync()
        {
            // Nothing to do
            return Task.CompletedTask;
        }

        public void ReceiveInitiativeCreated(Func<InitiativeCreatedEventArgs, CancellationToken, Task> handler, MessageHandlerOptions options)
        {
            CreatedHandlers.Add(handler);
        }

        public void ReceiveInitiativeCreated(Func<InitiativeCreatedEventArgs, CancellationToken, Task> handler, Func<ExceptionReceivedEventArgs, Task> exceptionReceivedHandler)
        {
            CreatedHandlers.Add(handler);
        }

        public void ReceiveInitiativeLogged(Func<InitiativeLoggedEventArgs, CancellationToken, Task> handler, MessageHandlerOptions options)
        {
            InitiativeLoggedHandlers.Add(handler);
        }

        public void ReceiveInitiativeWorkItemCreated(Func<WorkOrderCreatedEventArgs, CancellationToken, Task> handler, MessageHandlerOptions options)
        {
            WorkOrderCreatedHandlers.Add(handler);
        }

        public void ReceiveInitiativeWorkItemCreated(Func<WorkOrderCreatedEventArgs, CancellationToken, Task> handler, Func<ExceptionReceivedEventArgs, Task> exceptionReceivedHandler)
        {
            WorkOrderCreatedHandlers.Add(handler);
        }

        public void ReceiveWorkOrderUpdated(Func<WorkOrderUpdatedEventArgs, CancellationToken, Task> handler, MessageHandlerOptions options)
        {
            WorkOrderUpdatedHandlers.Add(handler);
        }
    }

}

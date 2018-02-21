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

        public void ReceiveMessages(
            Func<InitiativeCreatedEventArgs, CancellationToken, Task> initiativeCreatedHndler = null, 
            Func<WorkOrderCreatedEventArgs, CancellationToken, Task> workOrderCreatedHandler = null, 
            Func<WorkOrderUpdatedEventArgs, CancellationToken, Task> workOrderUpdatedHandler = null, 
            Func<InitiativeLoggedEventArgs, CancellationToken, Task> initiativeLoggedHandler = null, 
            MessageHandlerOptions options = null)
        {
            if (initiativeCreatedHndler != null)
                CreatedHandlers.Add(initiativeCreatedHndler);
            if (workOrderCreatedHandler != null)
                WorkOrderCreatedHandlers.Add(workOrderCreatedHandler);
            if (workOrderUpdatedHandler != null)
                WorkOrderUpdatedHandlers.Add(workOrderUpdatedHandler);
            if (initiativeLoggedHandler != null)
                InitiativeLoggedHandlers.Add(initiativeLoggedHandler);
        }
    }

}

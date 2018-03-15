using Microsoft.Azure.ServiceBus;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CoE.Ideas.Core.ServiceBus
{
    public class SynchronousInitiativeMessageReceiver : IInitiativeMessageReceiver
    {
        public SynchronousInitiativeMessageReceiver()
        {
            CreatedHandlers = new List<Func<InitiativeCreatedEventArgs, CancellationToken, Task>>();
            WorkOrderCreatedHandlers = new List<Func<WorkOrderCreatedEventArgs, CancellationToken, Task>>();
            WorkOrderUpdatedHandlers = new List<Func<WorkOrderUpdatedEventArgs, CancellationToken, Task>>();
            InitiativeLoggedHandlers = new List<Func<InitiativeLoggedEventArgs, CancellationToken, Task>>();
        }

        public IList<Func<InitiativeCreatedEventArgs, CancellationToken, Task>> CreatedHandlers { get; private set; }

        public IList<Func<WorkOrderCreatedEventArgs, CancellationToken, Task>> WorkOrderCreatedHandlers { get; private set; }
        public IList<Func<WorkOrderUpdatedEventArgs, CancellationToken, Task>> WorkOrderUpdatedHandlers { get; private set; }

        public IList<Func<InitiativeLoggedEventArgs, CancellationToken, Task>> InitiativeLoggedHandlers { get; private set; }


        public void ReceiveMessages(Func<InitiativeCreatedEventArgs, CancellationToken, Task> initiativeCreatedHandler = null,
            Func<WorkOrderCreatedEventArgs, CancellationToken, Task> workOrderCreatedHandler = null,
            Func<WorkOrderUpdatedEventArgs, CancellationToken, Task> workOrderUpdatedHandler = null,
            Func<InitiativeLoggedEventArgs, CancellationToken, Task> initiativeLoggedHandler = null,
            MessageHandlerOptions options = null)
        {
            if (initiativeCreatedHandler != null)
                CreatedHandlers.Add(initiativeCreatedHandler);
            if (workOrderCreatedHandler != null)
                WorkOrderCreatedHandlers.Add(workOrderCreatedHandler);
            if (workOrderUpdatedHandler != null)
                WorkOrderUpdatedHandlers.Add(workOrderUpdatedHandler);
            if (initiativeLoggedHandler != null)
                InitiativeLoggedHandlers.Add(initiativeLoggedHandler);
        }
    }
}

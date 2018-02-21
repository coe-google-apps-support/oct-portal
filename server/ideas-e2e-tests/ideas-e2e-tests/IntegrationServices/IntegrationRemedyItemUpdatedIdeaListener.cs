using CoE.Ideas.Core;
using CoE.Ideas.Core.ServiceBus;
using CoE.Ideas.Remedy.SbListener;
using Serilog;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CoE.Ideas.EndToEnd.Tests.IntegrationServices
{
    internal class IntegrationRemedyItemUpdatedIdeaListener : RemedyItemUpdatedIdeaListener
    {
        public IntegrationRemedyItemUpdatedIdeaListener(IUpdatableIdeaRepository ideaRepository, 
            IInitiativeMessageReceiver initiativeMessageReceiver, 
            ILogger logger) 
            : base(ideaRepository, initiativeMessageReceiver, logger)
        {
            WorkOrdersCreated = new List<WorkOrderCreatedEventArgs>();
            WorkOrdersUpdated = new List<WorkOrderUpdatedEventArgs>();
        }

        public ICollection<WorkOrderCreatedEventArgs> WorkOrdersCreated { get; private set; }
        public ICollection<WorkOrderUpdatedEventArgs> WorkOrdersUpdated { get; private set; }

        protected override async Task OnInitiativeWorkItemCreated(WorkOrderCreatedEventArgs args, CancellationToken token)
        {
            await base.OnInitiativeWorkItemCreated(args, token);
            WorkOrdersCreated.Add(args);
        }

        protected override async Task OnWorkOrderUpdatedAsync(WorkOrderUpdatedEventArgs args, CancellationToken token)
        {
            await base.OnWorkOrderUpdatedAsync(args, token);
            WorkOrdersUpdated.Add(args);
        }
    }
}

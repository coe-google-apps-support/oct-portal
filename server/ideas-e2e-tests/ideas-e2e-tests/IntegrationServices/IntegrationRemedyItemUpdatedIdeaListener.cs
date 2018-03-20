using CoE.Ideas.Core;
using CoE.Ideas.Core.Data;
using CoE.Ideas.Core.ServiceBus;
using CoE.Ideas.Core.Services;
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
        public IntegrationRemedyItemUpdatedIdeaListener(IInitiativeRepository ideaRepository,
            IPersonRepository personRepository,
            IInitiativeMessageReceiver initiativeMessageReceiver, 
            ILogger logger) 
            : base(ideaRepository, personRepository, initiativeMessageReceiver, logger)
        {
            WorkOrdersCreated = new List<WorkOrderCreatedEventArgs>();
            WorkOrdersUpdated = new List<Tuple<WorkOrderUpdatedEventArgs, Initiative>>();
        }

        public ICollection<WorkOrderCreatedEventArgs> WorkOrdersCreated { get; private set; }
        public ICollection<Tuple<WorkOrderUpdatedEventArgs, Initiative>> WorkOrdersUpdated { get; private set; }

        protected override async Task OnInitiativeWorkItemCreated(WorkOrderCreatedEventArgs args, CancellationToken token)
        {
            await base.OnInitiativeWorkItemCreated(args, token);
            WorkOrdersCreated.Add(args);
        }

        protected override async Task<Initiative> OnWorkOrderUpdatedAsync(WorkOrderUpdatedEventArgs args, CancellationToken token)
        {
            var idea = await base.OnWorkOrderUpdatedAsync(args, token);
            WorkOrdersUpdated.Add(new Tuple<WorkOrderUpdatedEventArgs, Initiative>(args, idea));
            return idea;
        }
    }
}

using CoE.Ideas.Core;
using CoE.Ideas.Core.ServiceBus;
using CoE.Ideas.Remedy;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CoE.Ideas.EndToEnd.Tests
{
    internal class IntegrationRemedyListenerNewIdeaListener : NewIdeaListener
    {
        public IntegrationRemedyListenerNewIdeaListener(IInitiativeMessageReceiver initiativeMessageReceiver,
            IInitiativeMessageSender initiativeMessageSender,
            IRemedyService remedyService,
            //IActiveDirectoryUserService activeDirectoryUserService,
            Serilog.ILogger logger) : base(initiativeMessageReceiver, initiativeMessageSender, remedyService, logger)
        {
            NewInitiatives = new List<Idea>();
        }

        public ICollection<Idea> NewInitiatives { get; private set; }


        protected override async Task OnNewInitiative(InitiativeCreatedEventArgs e, CancellationToken token)
        {
            await base.OnNewInitiative(e, token);
            NewInitiatives.Add(e.Initiative);
        }
    }
}

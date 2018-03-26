using CoE.Ideas.Core.Data;
using CoE.Ideas.Core.ServiceBus;
using CoE.Ideas.Remedy;
using CoE.Ideas.Shared.People;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CoE.Ideas.EndToEnd.Tests.IntegrationServices
{
    internal class IntegrationRemedyListenerNewIdeaListener : NewIdeaListener
    {
        public IntegrationRemedyListenerNewIdeaListener(IInitiativeMessageReceiver initiativeMessageReceiver,
            IInitiativeMessageSender initiativeMessageSender,
            IRemedyService remedyService,
            IPeopleService peopleService,
            //IActiveDirectoryUserService activeDirectoryUserService,
            Serilog.ILogger logger) : base(initiativeMessageReceiver, initiativeMessageSender, remedyService, peopleService, logger)
        {
            _logger = logger ?? throw new ArgumentNullException("logger");

            NewInitiatives = new List<Initiative>();
        }

        private readonly Serilog.ILogger _logger;

        public ICollection<Initiative> NewInitiatives { get; private set; }


        protected override async Task OnNewInitiative(InitiativeCreatedEventArgs e, CancellationToken token)
        {
            await base.OnNewInitiative(e, token);
            NewInitiatives.Add(e.Initiative);
        }
    }
}

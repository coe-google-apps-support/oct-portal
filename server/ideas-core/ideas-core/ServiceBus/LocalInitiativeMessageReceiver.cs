using System;
using System.Security.Claims;
using CoE.Ideas.Core.Services;
using EnsureThat;
using Microsoft.Azure.ServiceBus;

namespace CoE.Ideas.Core.ServiceBus
{
    internal class LocalInitiativeMessageReceiver : InitiativeMessageReceiver
    {
        public LocalInitiativeMessageReceiver(ISubscriptionClient subscriptionClient,
                Serilog.ILogger logger,
                IInitiativeRepository initiativeRepository) : base(subscriptionClient, logger)
        {
            EnsureArg.IsNotNull(initiativeRepository);
            _initiativeRepository = initiativeRepository;
        }

        private readonly IInitiativeRepository _initiativeRepository;

        protected override IInitiativeRepository GetInitiativeRepository(ClaimsPrincipal owner)
        {
            // local repository doesn't care about wheter
            return _initiativeRepository;
        }
    }
}

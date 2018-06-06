using System;
using System.Security.Claims;
using CoE.Ideas.Core.Services;
using EnsureThat;
using Microsoft.Azure.ServiceBus;

namespace CoE.Ideas.Core.ServiceBus
{
    internal class LocalInitiativeMessageReceiver : InitiativeMessageReceiver
    {
        public LocalInitiativeMessageReceiver(IMessageReceiver messageReceiver,
                Serilog.ILogger logger,
                IServiceProvider serviceProvider
                ) : base(messageReceiver, logger)
        {
            EnsureArg.IsNotNull(serviceProvider);
            _serviceProvider = serviceProvider;
        }

        private readonly IServiceProvider _serviceProvider;

        protected override IInitiativeRepository GetInitiativeRepository(ClaimsPrincipal owner)
        {
            // we use to just get the repository once, but in case it's defined as transient we'll 
            // as the service provider every time
            // note that "owner" is irrelevant for a local initiative repository
            return (IInitiativeRepository)_serviceProvider.GetService(typeof(IInitiativeRepository));
        }
    }
}

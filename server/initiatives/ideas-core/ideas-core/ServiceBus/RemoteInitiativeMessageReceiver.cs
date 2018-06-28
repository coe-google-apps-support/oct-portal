using CoE.Ideas.Core.Services;
using EnsureThat;
using Microsoft.Azure.ServiceBus;
using System;
using System.Security.Claims;

namespace CoE.Ideas.Core.ServiceBus
{
    internal class RemoteInitiativeMessageReceiver : InitiativeMessageReceiver
    {
        public RemoteInitiativeMessageReceiver(IMessageReceiver messageReceiver,
                Serilog.ILogger logger,
                IServiceProvider serviceProvider) : base(messageReceiver, logger)
        {
            EnsureArg.IsNotNull(serviceProvider);
            _serviceProvider = serviceProvider;
        }

        private readonly IServiceProvider _serviceProvider;

        protected override IInitiativeRepository GetInitiativeRepository(ClaimsPrincipal owner)
        {
            if (!(_serviceProvider.GetService(typeof(RemoteInitiativeRepository)) is RemoteInitiativeRepository remoteInitiativeRepository))
                throw new InvalidOperationException("Unable to find RemoteInitiativeRepository in ServiceProvider");
            remoteInitiativeRepository.SetUser(owner);
            return remoteInitiativeRepository;
        }
    }
}

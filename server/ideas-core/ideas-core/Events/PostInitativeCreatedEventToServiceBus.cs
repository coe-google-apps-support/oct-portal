using CoE.Ideas.Core.ServiceBus;
using CoE.Ideas.Core.Services;
using EnsureThat;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CoE.Ideas.Core.Events
{
    public class PostInitativeCreatedEventToServiceBus : INotificationHandler<InitiativeCreatedDomainEvent>
    {
        public PostInitativeCreatedEventToServiceBus(Serilog.ILogger logger,
            IInitiativeRepository initiativeRepository,
            IHttpContextAccessor httpContextAccessor,
            IInitiativeMessageSender initiativeMessageSender)
        {
            EnsureArg.IsNotNull(logger);
            EnsureArg.IsNotNull(initiativeRepository);
            EnsureArg.IsNotNull(httpContextAccessor);
            EnsureArg.IsNotNull(initiativeMessageSender);

            _logger = logger;
            _initiativeRepository = initiativeRepository;
            _httpContextAccessor = httpContextAccessor;
            _initiativeMessageSender = initiativeMessageSender;
        }

        private readonly Serilog.ILogger _logger;
        private readonly IInitiativeRepository _initiativeRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IInitiativeMessageSender _initiativeMessageSender;

        public async Task Handle(InitiativeCreatedDomainEvent notification, CancellationToken cancellationToken)
        {
            var initiative = await _initiativeRepository.GetInitiativeAsync(notification.InitiativeId);

            await _initiativeMessageSender.SendInitiativeCreatedAsync(new InitiativeCreatedEventArgs()
            {
                Initiative = initiative,
                Owner = _httpContextAccessor.HttpContext.User
            });
        }
    }
}

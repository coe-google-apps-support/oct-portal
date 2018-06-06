using CoE.Ideas.Core.Data;
using CoE.Ideas.Core.Events;
using CoE.Ideas.Core.ServiceBus;
using CoE.Ideas.Core.Services;
using EnsureThat;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CoE.Ideas.Core.EventHandlers
{
    public class CaptureInitiativeStatusChangedDomainEventHandler : INotificationHandler<InitiativeStatusChangedDomainEvent>
    {
        public CaptureInitiativeStatusChangedDomainEventHandler(Serilog.ILogger logger,
            IInitiativeRepository initiativeRepository,
            IHttpContextAccessor httpContextAccessor,
            IInitiativeMessageSender initiativeMessageSender)
        {
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

        public async Task Handle(InitiativeStatusChangedDomainEvent notification, CancellationToken cancellationToken)
        {
            EnsureArg.IsNotNull(notification);

            _logger.Debug("Initiative Status changed, will post message to service bus");
            var initiative = await _initiativeRepository.GetInitiativeAsync(notification.InitiativeId);
            if (initiative == null)
            {
                _logger.Error("Received new initiave event but couldn't get initiative with id {InitiativeUid}", notification.InitiativeId);
                throw new Exception($"Received new initiave event but couldn't get initiative with id {notification.InitiativeId}");
            }
            else
                _logger.Information("Posting NewInitiativeCreated event to service bus for Initiative {InitiativeId}", initiative.Id);

            await _initiativeMessageSender.SendInitiativeStatusDescriptionChangedAsync(new InitiativeStatusDescriptionChangedEventArgs()
            {
                Initiative = initiative,
                Owner = _httpContextAccessor.HttpContext.User
            });
        }
    }
}

using CoE.Ideas.Core.Data;
using CoE.Ideas.Core.Events;
using CoE.Ideas.Core.ServiceBus;
using CoE.Ideas.Core.Services;
using CoE.Ideas.Shared.Security;
using EnsureThat;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CoE.Ideas.Core.EventHandlers
{
    public class CaptureInitiativeStatusDescriptionChangedDomainEventHandler : INotificationHandler<InitiativeStatusDescriptionUpdatedDomainEvent>
    {
        public CaptureInitiativeStatusDescriptionChangedDomainEventHandler(Serilog.ILogger logger,
            IInitiativeRepository initiativeRepository,
            ICurrentUserAccessor currentUserAccessor,
            IInitiativeMessageSender initiativeMessageSender)
        {
            EnsureArg.IsNotNull(logger);
            EnsureArg.IsNotNull(initiativeRepository);
            EnsureArg.IsNotNull(currentUserAccessor);
            EnsureArg.IsNotNull(initiativeMessageSender);

            _logger = logger;
            _initiativeRepository = initiativeRepository;
            _currentUserAccessor = currentUserAccessor;
            _initiativeMessageSender = initiativeMessageSender;
        }

        private readonly Serilog.ILogger _logger;
        private readonly IInitiativeRepository _initiativeRepository;
        private readonly ICurrentUserAccessor _currentUserAccessor;
        private readonly IInitiativeMessageSender _initiativeMessageSender;


        public async Task Handle(InitiativeStatusDescriptionUpdatedDomainEvent notification, CancellationToken cancellationToken)
        {
            _logger.Debug("Initiative Status Description changed, will post message to service bus");
            var initiative = await _initiativeRepository.GetInitiativeAsync(notification.InitiativeId);
            if (initiative == null)
            {
                _logger.Error("Received new initiave event but couldn't get initiative with id {InitiativeUid}", notification.InitiativeId);
                throw new Exception($"Received new initiave event but couldn't get initiative with id {notification.InitiativeId}");
            }
            else
                _logger.Information("Posting StatusDescriptionChanged event to service bus for Initiative {InitiativeId}", initiative.Id);

            await _initiativeMessageSender.SendInitiativeStatusDescriptionChangedAsync(new InitiativeStatusDescriptionChangedEventArgs()
            {
                Initiative = initiative,
                Owner = _currentUserAccessor.User
            });

        }
    }
}

﻿using CoE.Ideas.Core.Events;
using CoE.Ideas.Core.ServiceBus;
using CoE.Ideas.Core.Services;
using CoE.Ideas.Shared.Security;
using EnsureThat;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CoE.Ideas.Core.EventHandlers
{
    public class PostInitativeCreatedEventToServiceBus : INotificationHandler<InitiativeCreatedDomainEvent>
    {
        public PostInitativeCreatedEventToServiceBus(Serilog.ILogger logger,
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

        public async Task Handle(InitiativeCreatedDomainEvent notification, CancellationToken cancellationToken)
        {
            _logger.Debug("New Initiative Created, will post message to service bus");
            try
            {
                var initiative = await _initiativeRepository.GetInitiativeAsync(notification.InitiativeId);
                if (initiative == null)
                {
                    _logger.Error("Received new initiave event but couldn't get initiative with id {InitiativeUid}", notification.InitiativeId);
                    throw new Exception($"Received new initiave event but couldn't get initiative with id {notification.InitiativeId}");
                }
                else
                    _logger.Information("Posting NewInitiativeCreated event to service bus for Initiative {InitiativeId}", initiative.Id);

                await _initiativeMessageSender.SendInitiativeCreatedAsync(new InitiativeCreatedEventArgs()
                {
                    Initiative = initiative,
                    Owner = _currentUserAccessor.User,
                    SkipEmailNotification = notification.SkipEmailNotification
                });
            }
            catch (Exception err)
            {
                _logger.Error(err, "Error posting Initative Created event to Service Bus: {ErrorMessage}", err.Message);
            }
        }
    }
}

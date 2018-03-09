using CoE.Ideas.Core.ServiceBus;
using CoE.Ideas.Core.Services;
using CoE.Ideas.Core.WordPress;
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
    public class CreateWordPressPostWhenInitiativeCreatedDomainEventHandler : INotificationHandler<InitiativeCreatedDomainEvent>
    {
        public CreateWordPressPostWhenInitiativeCreatedDomainEventHandler(Serilog.ILogger logger,
            IWordPressClient wordPressClient,
            IInitiativeRepository initiativeRepository,
            IInitiativeMessageSender initiativeMessageSender,
            IHttpContextAccessor httpContextAccessor)
        {
            EnsureArg.IsNotNull(logger);
            EnsureArg.IsNotNull(wordPressClient);
            EnsureArg.IsNotNull(initiativeRepository);
            EnsureArg.IsNotNull(initiativeMessageSender);
            EnsureArg.IsNotNull(httpContextAccessor);

            _logger = logger;
            _wordPressClient = wordPressClient;
            _initiativeRepository = initiativeRepository;
            _initiativeMessageSender = initiativeMessageSender;
            _httpContextAccessor = httpContextAccessor;
        }

        private readonly Serilog.ILogger _logger;
        private readonly IWordPressClient _wordPressClient;
        private readonly IInitiativeRepository _initiativeRepository;
        private readonly IInitiativeMessageSender _initiativeMessageSender;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public async Task Handle(InitiativeCreatedDomainEvent notification, CancellationToken cancellationToken)
        {
            EnsureArg.IsNotNull(notification);

            // post to WordPress
            var newPost = await _wordPressClient.PostIdeaAsync(notification.Initiative);

            _logger.Information("Posting to service bus");
            await _initiativeMessageSender.SendInitiativeCreatedAsync(
                new InitiativeCreatedEventArgs()
                {
                    Initiative = notification.Initiative,
                    Owner = _httpContextAccessor.HttpContext.User
                });
            _logger.Information("Posted to service bus");


        }
    }
}

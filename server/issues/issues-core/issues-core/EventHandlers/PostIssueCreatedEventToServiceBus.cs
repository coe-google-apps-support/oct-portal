using CoE.Issues.Core.Event;
using MediatR;
using System;
using CoE.Issues.Core.ServiceBus;
using CoE.Issues.Core.Services;
using CoE.Ideas.Shared.Security;
using EnsureThat;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CoE.Issues.Core.EventHandlers
{
    public class PostIssueCreatedEventToServiceBus : INotificationHandler<IssueNewCreatedDomainEvent>
    {
        public PostIssueCreatedEventToServiceBus(Serilog.ILogger logger,
           IIssueRepository issueRepository,
           ICurrentUserAccessor currentUserAccessor,
           IIssueMessageSender issueMessageSender)
        {
            EnsureArg.IsNotNull(logger);
            EnsureArg.IsNotNull(issueRepository);
            EnsureArg.IsNotNull(currentUserAccessor);
            EnsureArg.IsNotNull(issueMessageSender);

            _logger = logger;
            _issueRepository = issueRepository;
            _currentUserAccessor = currentUserAccessor;
            _issueMessageSender = issueMessageSender;
        }

        private readonly Serilog.ILogger _logger;
        private readonly IIssueRepository _issueRepository;
        private readonly ICurrentUserAccessor _currentUserAccessor;
        private readonly IIssueMessageSender _issueMessageSender;

        public async Task Handle(IssueNewCreatedDomainEvent notification, CancellationToken cancellationToken)
        {
            _logger.Debug("New Issue Created, will post message to service bus");
            try
            {
                var issue = await _issueRepository.GetIssueAsync(notification.IssueId);
                if (issue == null)
                {
                    _logger.Error("Received new initiave event but couldn't get issue with id {IssueUid}", notification.IssueId);
                    throw new Exception($"Received new initiave event but couldn't get issue with id {notification.IssueId}");
                }
                else
                    _logger.Information("Posting NewIssueCreated event to service bus for Issue {IssueId}", issue.Id);

                await _issueMessageSender.SendNewIssueCreatedAsync(new NewIssueCreatedEventArgs()
                {
                    Issue = issue,
                    Owner = _currentUserAccessor.User,
                });
            }
            catch (Exception err)
            {
                _logger.Error(err, "Error posting Initative Created event to Service Bus: {ErrorMessage}", err.Message);
            }
        }
    }
}
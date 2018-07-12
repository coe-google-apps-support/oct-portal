using CoE.Issues.Core.Data;
using CoE.Issues.Core.ServiceBus;
using CoE.Issues.Core.Services;
using EnsureThat;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Serilog.Context;
using CoE.Ideas.Shared.WordPress;

namespace CoE.Issues.Remedy.SbListener
{
    public class RemedyIssueCreatedListener
    {
        public RemedyIssueCreatedListener(
            IIssueMessageReceiver issueMessageReceiver,
            IIssueRepository issueRepository,
            IWordPressRepository userRepository,
            Serilog.ILogger logger)
        {
            EnsureArg.IsNotNull(issueRepository);
            EnsureArg.IsNotNull(issueMessageReceiver);
            EnsureArg.IsNotNull(logger);
            _issueRepository = issueRepository ?? throw new ArgumentException("issueRepository");
            _issueMessageReceiver = issueMessageReceiver?? throw new ArgumentException("issueMessageReceiver");
            _logger = logger ?? throw new ArgumentException("logger");

            issueMessageReceiver.ReceiveMessages(
                issueCreatedHandler: OnIssueCreated);
        }

        private readonly IIssueRepository _issueRepository;
        private readonly Serilog.ILogger _logger;
        private readonly IIssueMessageReceiver _issueMessageReceiver;


        protected virtual async Task OnIssueCreated(IssueCreatedEventArgs args, CancellationToken token)
        {
            // TODO: add logging and exception handling
            _logger.Information("entering the sblistener");

            // see if the issue already exists
            Issue issue = null; // _issueRepository.GetIssueByIncidentId
            if (issue == null)
            {
                issue = Issue.Create(args.Incident.DESCRIPTION, args.Incident.DETAILED_DECRIPTION);

                // fill in any other fields here
                // save to database
                await _issueRepository.AddIssueAsync(issue, token);
            }
            else
            {
                // TODO: update the issue with any relevant updated fields

                await _issueRepository.UpdateIssueAsync(issue);
            }


        }
    }
}


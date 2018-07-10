using CoE.Issues.Core.Data;
using CoE.Issues.Core.ServiceBus;
using CoE.Issues.Core.Services;
using EnsureThat;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CoE.Issues.Remedy.SbListener
{
    public class RemedyIssueCreatedListener
    {
        public RemedyIssueCreatedListener(IIssueRepository issueRepository)
        {
            EnsureArg.IsNotNull(issueRepository);
            _issueRepository = issueRepository;
        }

        private readonly IIssueRepository _issueRepository;

        protected virtual async Task OnIssueCreated(IssueCreatedEventArgs args, CancellationToken token)
        {
            // TODO: add logging and exception handling

            var issue = Issue.Create(args.Title, args.Description);

            // fill in any other fields here

            // save to database
            await _issueRepository.AddIssueAsync(issue, token);
        }
    }
}

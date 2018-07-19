using CoE.Ideas.Shared.People;
using CoE.Issues.Core.ServiceBus;
using EnsureThat;
using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using CoE.Issues.Core.Data;
using Serilog.Context;
using CoE.Ideas.Shared.Security;

namespace CoE.Issues.Remedy
{
    public class NewIssueListener
    {
        public NewIssueListener(IIssueMessageReceiver IssueMessageReceiver,
            IIssueMessageSender IssueMessageSender,
            IRemedyService remedyService,
            IPeopleService peopleService,
            Serilog.ILogger logger)
        {
            EnsureArg.IsNotNull(IssueMessageReceiver);
            EnsureArg.IsNotNull(IssueMessageSender);
            EnsureArg.IsNotNull(remedyService);
            EnsureArg.IsNotNull(peopleService);
            EnsureArg.IsNotNull(logger);

            _IssueMessageReceiver = IssueMessageReceiver;
            _IssueMessageSender = IssueMessageSender;
            _remedyService = remedyService;
            _peopleService = peopleService;
            _logger = logger ?? throw new ArgumentNullException("logger");

            _logger.Information("Starting messsage pump for New Issues");
            IssueMessageReceiver.ReceiveMessages(newissueCreatedHandler: OnNewIssue);
        }

        private readonly IIssueMessageReceiver _IssueMessageReceiver;
        private readonly IIssueMessageSender _IssueMessageSender;
        private readonly IRemedyService _remedyService;
        private readonly IPeopleService _peopleService;
        private readonly Serilog.ILogger _logger;

        protected virtual Task OnError(ExceptionReceivedEventArgs err)
        {
            _logger.Error(err.Exception, "Error receiving message: {ErrorMessage}", err.Exception.Message);
            return Task.CompletedTask;
        }

        protected virtual async Task OnNewIssue(NewIssueCreatedEventArgs issueData, CancellationToken token)
        {
            var newIssue = issueData.Issue;
            var newOwner = issueData.Owner;

            using (LogContext.PushProperty("IssueId", newIssue.Id))
            {
                Stopwatch watch = new Stopwatch();
                watch.Start();

                PersonData personData;
                try
                {
                    personData = await GetPersonData(newOwner.GetEmail());
                }
                catch (Exception err)
                {
                    _logger.Error(err, "Unable to get PersonData for {IssueId} from {EmailAddress}: {ErrorMessage}", newIssue.Id, newOwner.GetEmail(), err.Message);
                    throw;
                }

                string IncidentId;
                try
                {
                    IncidentId = await CreateIncident(newIssue, personData);
                }
                catch (Exception err)
                {
                    _logger.Error(err, "Unable to get create a Incident for Issue {IssueId} from {EmailAddress}: {ErrorMessage}", newIssue.Id, newOwner.GetEmail(), err.Message);
                    throw;
                }


                await SendIncidentCreatedMessage(newIssue, newOwner, IncidentId);
                _logger.Information("Processed OnNewIssue for Issue {IssueId} in {ElapsedMilliseconds}ms", newIssue.Id, watch.ElapsedMilliseconds);

            }
        }

        protected virtual async Task<PersonData> GetPersonData(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return null;

            Stopwatch watch = new Stopwatch();
            try
            {
                var returnValue = await _peopleService.GetPersonByEmailAsync(email);
                _logger.Information("Retrieved details about user {EmailAddress} in {ElapsedMilliseconds}ms", email, watch.ElapsedMilliseconds);
                return returnValue;
            }
            catch (Exception err)
            {
                _logger.Error(err, "Unable to get information about user with email {EmailAddress}", email);
                return null;
            }
        }

        protected virtual async Task<string> CreateIncident(Issue Issue, PersonData personData)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            string remedyTicketId = null;
            remedyTicketId = await _remedyService.PostNewissueAsync(Issue,
                personData);
            _logger.Information("Created Remedy Incident in {ElapsedMilliseconds}ms. Issue Id {IssueId}, IncidentId {IncidentId}", watch.ElapsedMilliseconds, Issue.Id, remedyTicketId);
            return remedyTicketId;
        }

        protected virtual async Task SendIncidentCreatedMessage(Issue issue, ClaimsPrincipal owner, string incidentId)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();

            await _IssueMessageSender.SendNewIssueCreatedAsync(
                new NewIssueCreatedEventArgs()
                {
                    Issue = issue,
                    Owner = owner,
                    IncidentId = incidentId
                });

            _logger.Information("Send remedy Incident created message to service bus in {ElapsedMilliseconds}ms", watch.ElapsedMilliseconds);
        }
    }
}

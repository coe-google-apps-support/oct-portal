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

namespace CoE.Issues.Remedy
{
    public class NewIssueListener
    {
        public NewIssueListener(IIssueMessageReceiver IssueMessageReceiver,
            IIssueMessageSender IssueMessageSender,
            IIssueService IssueService,
            IRemedyService remedyService,
            IPeopleService peopleService,
            Serilog.ILogger logger)
        {
            EnsureArg.IsNotNull(IssueMessageReceiver);
            EnsureArg.IsNotNull(IssueMessageSender);
            EnsureArg.IsNotNull(IssueService);
            EnsureArg.IsNotNull(remedyService);
            EnsureArg.IsNotNull(peopleService);
            EnsureArg.IsNotNull(logger);

            _IssueMessageReceiver = IssueMessageReceiver;
            _IssueMessageSender = IssueMessageSender;
            _IssueService = IssueService;
            _remedyService = remedyService;
            _peopleService = peopleService;
            _logger = logger ?? throw new ArgumentNullException("logger");

            _logger.Information("Starting messsage pump for New Issues");
            IssueMessageReceiver.ReceiveMessages(IssueCreatedHandler: OnNewIssue);
        }

        private readonly IIssueMessageReceiver _IssueMessageReceiver;
        private readonly IIssueMessageSender _IssueMessageSender;
        private readonly IIssueService _IssueService;
        private readonly IRemedyService _remedyService;
        private readonly IPeopleService _peopleService;
        private readonly Serilog.ILogger _logger;

        protected virtual Task OnError(ExceptionReceivedEventArgs err)
        {
            _logger.Error(err.Exception, "Error receiving message: {ErrorMessage}", err.Exception.Message);
            return Task.CompletedTask;
        }

        protected virtual async Task OnNewIssue(IssueCreatedEventArgs e, CancellationToken token)
        {
            var Issue = e.Issue;
            var owner = e.Owner;
            using (LogContext.PushProperty("IssueId", Issue.Id))
            {
                _logger.Information("Begin OnNewIssue for Issue {IssueId}", Issue.Id);
                Stopwatch watch = new Stopwatch();
                watch.Start();

                PersonData personData;
                try
                {
                    personData = await GetPersonData(owner.GetEmail());
                }
                catch (Exception err)
                {
                    _logger.Error(err, "Unable to get PersonData for {IssueId} from {EmailAddress}: {ErrorMessage}", Issue.Id, owner.GetEmail(), err.Message);
                    throw;
                }

                string workOrderId;
                try
                {
                    workOrderId = await CreateWorkOrder(Issue, personData);
                }
                catch (Exception err)
                {
                    _logger.Error(err, "Unable to get create a workOrder for Issue {IssueId} from {EmailAddress}: {ErrorMessage}", Issue.Id, owner.GetEmail(), err.Message);
                    throw;
                }


                await SendWorkOrderCreatedMessage(Issue, owner, workOrderId, e.SkipEmailNotification);
                _logger.Information("Processed OnNewIssue for Issue {IssueId} in {ElapsedMilliseconds}ms", Issue.Id, watch.ElapsedMilliseconds);

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

        protected virtual async Task<string> CreateWorkOrder(Issue Issue, PersonData personData)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            string remedyTicketId = null;
            remedyTicketId = await _remedyService.PostNewIdeaAsync(Issue,
                personData,
                _IssueService.GetIssueUrl(Issue.Id));
            _logger.Information("Created Remedy Work Order in {ElapsedMilliseconds}ms. Issue Id {IssueId}, WorkOrderId {WorkOrderId}", watch.ElapsedMilliseconds, Issue.Id, remedyTicketId);
            return remedyTicketId;
        }

        protected virtual async Task SendWorkOrderCreatedMessage(Issue Issue, ClaimsPrincipal owner, string workOrderId, bool skipEmailNotification)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();

            await _IssueMessageSender.SendIssueWorkOrderCreatedAsync(
                new WorkOrderCreatedEventArgs()
                {
                    Issue = Issue,
                    Owner = owner,
                    WorkOrderId = workOrderId,
                    EtaUtc = etaUtc,
                    SkipEmailNotification = skipEmailNotification
                });

            _logger.Information("Send remedy work order created message to service bus in {ElapsedMilliseconds}ms", watch.ElapsedMilliseconds);
        }
    }
}

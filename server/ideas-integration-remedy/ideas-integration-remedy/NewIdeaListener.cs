using System;
using System.Diagnostics;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using CoE.Ideas.Core.Data;
using CoE.Ideas.Core.ServiceBus;
using CoE.Ideas.Shared.People;
using CoE.Ideas.Shared.Security;
using EnsureThat;
using Microsoft.Azure.ServiceBus;
using Serilog.Context;

namespace CoE.Ideas.Remedy
{
    public class NewIdeaListener
    {
        public NewIdeaListener(IInitiativeMessageReceiver initiativeMessageReceiver,
            IInitiativeMessageSender initiativeMessageSender,
            IRemedyService remedyService,
            IPeopleService peopleService,
            Serilog.ILogger logger) 
        {
            EnsureArg.IsNotNull(initiativeMessageReceiver);
            EnsureArg.IsNotNull(initiativeMessageSender);
            EnsureArg.IsNotNull(remedyService);
            EnsureArg.IsNotNull(peopleService);
            EnsureArg.IsNotNull(logger);

            _initiativeMessageReceiver = initiativeMessageReceiver;
            _initiativeMessageSender = initiativeMessageSender;
            _remedyService = remedyService;
            _peopleService = peopleService;
            _logger = logger ?? throw new ArgumentNullException("logger");

            _logger.Information("Starting messsage pump for New Initiatives");
            initiativeMessageReceiver.ReceiveMessages(initiativeCreatedHandler: OnNewInitiative);
        }

        private readonly IInitiativeMessageReceiver _initiativeMessageReceiver;
        private readonly IInitiativeMessageSender _initiativeMessageSender;
        private readonly IRemedyService _remedyService;
        private readonly IPeopleService _peopleService;
        private readonly Serilog.ILogger _logger;

        protected virtual Task OnError(ExceptionReceivedEventArgs err)
        {
            _logger.Error(err.Exception, "Error receiving message: {ErrorMessage}", err.Exception.Message);
            return Task.CompletedTask;
        }

        protected virtual async Task OnNewInitiative(InitiativeCreatedEventArgs e, CancellationToken token)
        {
            var initiative = e.Initiative;
            var owner = e.Owner;
            using (LogContext.PushProperty("InitiativeId", initiative.Id))
            {
                _logger.Information("Begin OnNewInitiative for initiative {InitiativeId}", initiative.Id);
                Stopwatch watch = new Stopwatch();
                watch.Start();

                var personData = await GetPersonData(owner.GetEmail());
                string workOrderId = await CreateWorkOrder(initiative, personData);
                await SendWorkOrderCreatedMessage(initiative, owner, workOrderId);

                _logger.Information("Processed OnNewInitiative for initiative {InitiativeId} in {ElapsedMilliseconds}ms", initiative.Id, watch.ElapsedMilliseconds);
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

        protected virtual async Task<string> CreateWorkOrder(Initiative initiative, PersonData personData)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            string remedyTicketId = null;
            remedyTicketId = await _remedyService.PostNewIdeaAsync(initiative, personData);
            _logger.Information("Created Remedy Work Order in {ElapsedMilliseconds}ms. Initiative Id {InitiativeId}, WorkOrderId {WorkOrderId}", watch.ElapsedMilliseconds, initiative.Id, remedyTicketId);
            return remedyTicketId;
        }

        protected virtual async Task SendWorkOrderCreatedMessage(Initiative initiative, ClaimsPrincipal owner, string workOrderId)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();

            await _initiativeMessageSender.SendInitiativeWorkOrderCreatedAsync(
                new WorkOrderCreatedEventArgs()
                {
                    Initiative = initiative,
                    Owner = owner,
                    WorkOrderId = workOrderId
                });

            _logger.Information("Send remedy work order created message to service bus in {ElapsedMilliseconds}ms", watch.ElapsedMilliseconds);
        }
    }
}

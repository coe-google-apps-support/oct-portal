using CoE.Ideas.Core;
using CoE.Ideas.Core.ServiceBus;
using CoE.Ideas.Core.WordPress;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Logging;
using Serilog.Context;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.DirectoryServices.AccountManagement;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CoE.Ideas.Integration.Logger
{
    public class NewIdeaListener
    {
        public NewIdeaListener(IIdeaLogger ideaLogger,
            //IActiveDirectoryUserService activeDirectoryUserService,
            IInitiativeMessageSender initiativeMessageSender,
            IInitiativeMessageReceiver initiativeMessageReceiver,
            Serilog.ILogger logger)
        {
            _ideaLogger = ideaLogger ?? throw new ArgumentNullException("ideaLogger") ;
            //_activeDirectoryUserService = activeDirectoryUserService;
            _initiativeMessageSender = initiativeMessageSender ?? throw new ArgumentNullException("initiativeMessageSender");
            _initiativeMessageReceiver = initiativeMessageReceiver ?? throw new ArgumentNullException("initiativeMessageReceiver");
            _logger = logger ?? throw new ArgumentNullException("logger");

            _initiativeMessageReceiver.ReceiveInitiativeWorkItemCreated(OnInitiativeWorkItemCreated,
                new MessageHandlerOptions(OnError)
                {
                    MaxConcurrentCalls = 30
                });
        }

        private readonly IIdeaLogger _ideaLogger;
        //private readonly IActiveDirectoryUserService _activeDirectoryUserService;
        private readonly IInitiativeMessageSender _initiativeMessageSender;
        private readonly IInitiativeMessageReceiver _initiativeMessageReceiver;
        private readonly Serilog.ILogger _logger;

        protected virtual Task OnError(ExceptionReceivedEventArgs err)
        {
            _logger.Error(err.Exception, "Error receiving message");
            return Task.CompletedTask;
        }

 
        protected virtual async Task OnInitiativeWorkItemCreated(WorkOrderCreatedEventArgs args, CancellationToken token)
        {
            if (args == null)
                throw new ArgumentNullException("args");

            var idea = args.Initiative;
            var owner = args.Owner;

            if (idea == null)
                throw new ArgumentException("Initiative cannot be null");
            if (owner == null)
                throw new ArgumentNullException("Owner cannot be null");
            var email = owner.GetEmail();
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentOutOfRangeException("Initiative Owner Email cannot be empty");

            var watch = new Stopwatch();
            watch.Start();

            using (LogContext.PushProperty("InitiativeId", idea.Id))
            {
                _logger.Information("Received message that a new initiative has been created with id {InitiativeId}");

                var loggerResponse = await LogNewInitiative(idea, owner);
                await SendInitiativeLoggerMessage(idea, owner, loggerResponse);
                watch.Stop();
                _logger.Information("Logger processed message in {ElapsedMilliseconds}ms", watch.ElapsedMilliseconds);
            }

        }

        protected virtual async Task<Google.Apis.Sheets.v4.Data.AppendValuesResponse> LogNewInitiative(Idea initiative, ClaimsPrincipal owner)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();

            Google.Apis.Sheets.v4.Data.AppendValuesResponse loggerResponse;
            try
            {
                loggerResponse = await _ideaLogger.LogIdeaAsync(initiative, owner, adUser: null);
                _logger.Information("Logged initiative in {ElapsedMilliseconds}ms", watch.ElapsedMilliseconds);
            }
            catch (Exception err)
            {
                _logger.Error(err, "Unable to log initiative {InitiativeId}: {ErrorMessage}", initiative.Id, err.Message);
                loggerResponse = null;
            }

            return loggerResponse;
        }

        protected virtual async Task SendInitiativeLoggerMessage(Idea initiative, 
            ClaimsPrincipal owner, 
            Google.Apis.Sheets.v4.Data.AppendValuesResponse loggerResponse)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();

            try
            {
                await _initiativeMessageSender.SendInitiativeLoggedAsync(new InitiativeLoggedEventArgs()
                {
                    Initiative = initiative,
                    Owner = owner,
                    RangeUpdated = loggerResponse?.TableRange
                });
                _logger.Information("Sent message on Service Bus that initiative has been logged in {ElapsedMilliseconds}ms", watch.ElapsedMilliseconds);
            }
            catch (Exception err)
            {
                Trace.TraceError($"Unable to send IdeaLogged message: { err.Message }");
                _logger.Error(err, "Unable to send message on Service Bus that initiative {InitiativeId}: {ErrorMessage}", initiative.Id, err.Message);
            }
        }
    }
}

﻿using CoE.Ideas.Shared.People;
using CoE.Ideas.Shared.ServiceBus;
using CoE.Issues.Core.Data;
using EnsureThat;
using Microsoft.Azure.ServiceBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CoE.Issues.Core.ServiceBus
{
     internal abstract class IssueMessageReceiver : IIssueMessageReceiver
    {

        public IssueMessageReceiver(IMessageReceiver messageReceiver,
            Serilog.ILogger logger)
        {
            EnsureArg.IsNotNull(messageReceiver);
            EnsureArg.IsNotNull(logger);
            _messageReceiver = messageReceiver;
            _logger = logger;
        }
        private readonly IMessageReceiver _messageReceiver;
        private readonly Serilog.ILogger _logger;


        public void ReceiveMessages(Func<IssueCreatedEventArgs, CancellationToken, Task> issueCreatedHandler = null,
            MessageHandlerOptions options = null)
        {
            var messageHandlerOptions = options ?? new MessageHandlerOptions(OnDefaultError);
            _messageReceiver.RegisterMessageHandler(async (msg, token) =>
            {
                _logger.Information("Received service bus message {MessageId}: {Label}", msg.Id.ToString(), msg.Label);


            switch (msg.Label)
            {
                case IssueMessageSender.ISSUE_CREATED:
                    {
                        if (issueCreatedHandler != null)
                            await ReceiveIssueCreated(msg, token, issueCreatedHandler);
                        else
                            await _messageReceiver.CompleteAsync(msg.LockToken);
                        break;
                    }
                    default:
                        {
                            await _messageReceiver.DeadLetterAsync(msg.LockToken, $"Unknown message type: { msg.Label }");
                            break;
                        }
                }
            }, messageHandlerOptions);
        }

        private async Task ReceiveIssueCreated(Ideas.Shared.ServiceBus.Message msg, CancellationToken token, Func<IssueCreatedEventArgs, CancellationToken, Task> issueCreatedHandler)
        {

            // TODO: add logging and error handling
            var args = new IssueCreatedEventArgs();
            if (msg.MessageProperties.ContainsKey("Title")) args.Title = msg.MessageProperties["Title"] as string;
            if (msg.MessageProperties.ContainsKey("Description")) args.Description = msg.MessageProperties["Description"] as string;
            if (msg.MessageProperties.ContainsKey("RemedyStatus")) args.RemedyStatus = msg.MessageProperties["RemedyStatus"] as string;
            if (msg.MessageProperties.ContainsKey("ReferenceId")) args.ReferenceId = msg.MessageProperties["ReferenceId"] as string;
            if (msg.MessageProperties.ContainsKey("AssigneeEmail")) args.AssigneeEmail = msg.MessageProperties["AssigneeEmail"] as string;
            if (msg.MessageProperties.ContainsKey("RequestorEmail")) args.RequestorEmail = msg.MessageProperties["RequestorEmail"] as string;
            if (msg.MessageProperties.ContainsKey("RequestorTelephone")) args.RequestorTelephone = msg.MessageProperties["RequestorTelephone"] as string;
            if (msg.MessageProperties.ContainsKey("RequestorGivenName")) args.RequestorGivenName = msg.MessageProperties["RequestorGivenName"] as string;
            if (msg.MessageProperties.ContainsKey("RequestorSurnName")) args.RequestorSurnName = msg.MessageProperties["RequestorSurnName"] as string;
            if (msg.MessageProperties.ContainsKey("RequestorDisplayName")) args.RequestorDisplayName = msg.MessageProperties["RequestorDisplayName"] as string;
            //if (msg.MessageProperties.ContainsKey("CreatedDate")) args.CreatedDate = msg.MessageProperties["CreatedDate"] as DateTime;

            

            // call the handler registered for this event
            await issueCreatedHandler(args, token);
        }

        protected virtual Task OnDefaultError(ExceptionReceivedEventArgs err)
        {
            _logger.Error(err.Exception, "Error receiving message: {ErrorMessage}", err.Exception.Message);
            return Task.CompletedTask;
        }


    }
}
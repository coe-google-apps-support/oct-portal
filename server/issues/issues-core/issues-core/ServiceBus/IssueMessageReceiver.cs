using CoE.Ideas.Shared.ServiceBus;
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
            var args = new IssueCreatedEventArgs()
            {
                Title = msg.MessageProperties["Title"] as string,
                Description = msg.MessageProperties["Description"] as string,
                RemedyStatus = msg.MessageProperties["RemedyStatus"] as string,
                RequestorName = msg.MessageProperties["RequestorName"] as string,
                ReferenceId = msg.MessageProperties["ReferenceId"] as string,
                AssigneeEmail = msg.MessageProperties["AssigneeEmail"] as string,
                CreatedDate = DateTime.Parse(msg.MessageProperties["CreatedDate"].ToString())
        };

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

using CoE.Ideas.Shared.ServiceBus;
using EnsureThat;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CoE.Issues.Core.ServiceBus
{
    internal class IssueMessageReceiver : IIssueMessgeReceiver
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
            Microsoft.Azure.ServiceBus.MessageHandlerOptions options = null)
        {
            var messageHandlerOptions = options ?? new Microsoft.Azure.ServiceBus.MessageHandlerOptions(OnDefaultError);
            _messageReceiver.RegisterMessageHandler(async (msg, token) =>
            {
                _logger.Information("Received service bus message {MessageId}: {Label}", msg.Id.ToString(), msg.Label);


            switch (msg.Label)
            {
                case IssueMessageSender.ISSUE_CREATED:
                    {
                        if (issueCreatedHandler != null)
                            await ReceiveInitiativeCreated(msg, token, issueCreatedHandler);
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

        private async Task ReceiveInitiativeCreated(Message msg, CancellationToken token, Func<IssueCreatedEventArgs, CancellationToken, Task> issueCreatedHandler)
        {
            // TODO: add logging and error handling
            var args = new IssueCreatedEventArgs()
            {
                Title = msg.MessageProperties["Title"] as string,
                Description = msg.MessageProperties["Description"] as string,
                RemedyStatus = msg.MessageProperties["RemedyStatus"] as string,
                RequestorEmail = msg.MessageProperties["RequestorEmail"] as string,
                ReferenceId = msg.MessageProperties["ReferenceId"] as string,
                AssigneeEmail = msg.MessageProperties["AssigneeEmail"] as string
            };

            // call the handler registered for this event
            await issueCreatedHandler(args, token);
        }

        protected virtual Task OnDefaultError(Microsoft.Azure.ServiceBus.ExceptionReceivedEventArgs err)
        {
            _logger.Error(err.Exception, "Error receiving message: {ErrorMessage}", err.Exception.Message);
            return Task.CompletedTask;
        }


    }
}

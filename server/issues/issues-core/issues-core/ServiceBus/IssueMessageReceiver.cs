using CoE.Ideas.Shared.People;
using CoE.Ideas.Shared.Security;
using CoE.Ideas.Shared.ServiceBus;
using CoE.Issues.Core.Data;
using CoE.Issues.Core.Services;
using EnsureThat;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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

        protected abstract IIssueRepository GetIssueRepository(ClaimsPrincipal owner);


        public void ReceiveMessages(Func<IssueCreatedEventArgs, CancellationToken, Task> issueCreatedHandler = null,
            Func<NewIssueCreatedEventArgs, CancellationToken, Task> newissueCreatedHandler = null,
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
                            {
                                _logger.Information("ISSUE_CREATED");
                                await ReceiveIssueCreated(msg, token, issueCreatedHandler);
                            }
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
            if (msg.MessageProperties.ContainsKey("AssigneeGroup")) args.AssigneeGroup = msg.MessageProperties["AssigneeGroup"] as string;
            if (msg.MessageProperties.ContainsKey("RequestorEmail")) args.RequestorEmail = msg.MessageProperties["RequestorEmail"] as string;
            if (msg.MessageProperties.ContainsKey("RequestorTelephone")) args.RequestorTelephone = msg.MessageProperties["RequestorTelephone"] as string;
            if (msg.MessageProperties.ContainsKey("RequestorGivenName")) args.RequestorGivenName = msg.MessageProperties["RequestorGivenName"] as string;
            if (msg.MessageProperties.ContainsKey("RequestorSurnName")) args.RequestorSurnName = msg.MessageProperties["RequestorSurnName"] as string;
            if (msg.MessageProperties.ContainsKey("RequestorDisplayName")) args.RequestorDisplayName = msg.MessageProperties["RequestorDisplayName"] as string;
            if (msg.MessageProperties.ContainsKey("Urgency")) args.Urgency = msg.MessageProperties["Urgency"] as string;
            var createdDate = await GetMessageProperty<DateTime>(msg, propertyName: nameof(IssueCreatedEventArgs.CreatedDate));
            args.CreatedDate = createdDate.Item;

            await issueCreatedHandler(args, token);
        }


        private async Task<bool> EnsureMessageLabel(Ideas.Shared.ServiceBus.Message message, string label)
        {
            if (message.Label == label)
                return true;
            else
            {
                await _messageReceiver.DeadLetterAsync(message.LockToken, $"Label was unexpected. Expected '{ label }', got '{ message.Label }';");
                return false;
            }
        }
        protected virtual Task OnDefaultError(ExceptionReceivedEventArgs err)
        {
            _logger.Error(err.Exception, "Error receiving message: {ErrorMessage}", err.Exception.Message);
            return Task.CompletedTask;
        }

        protected virtual async Task<GetItemResult<T>> GetMessageProperty<T>(Ideas.Shared.ServiceBus.Message message, string propertyName, bool allowNull = false)
        {
            if (message == null)
                throw new ArgumentNullException("msg");

            var result = new GetItemResult<T>();
            if (!message.MessageProperties.ContainsKey(propertyName))
            {
                string errorMessage = $"{ propertyName } not found in message";
                result.SetMessageDeadLettered(errorMessage);
                await _messageReceiver.DeadLetterAsync(message.LockToken, errorMessage);
            }
            else
            {
                object propertyObj = message.MessageProperties[propertyName];
                if (propertyObj == null)
                {
                    if (!allowNull)
                    {
                        string errorMessage = $"{ propertyName } was null";
                        _logger.Error("{PropertyName} was empty in Service Bus message", propertyName);
                        result.SetMessageDeadLettered(errorMessage);
                        await _messageReceiver.DeadLetterAsync(message.LockToken, errorMessage);
                    }
                    // else return null (or default) and don't dead letter
                }
                else
                {
                    try
                    {
                        result.Item = (T)propertyObj;
                    }
                    catch (Exception)
                    {
                        string errorMessage = $"{ propertyName } was not of type { typeof(T).FullName }";
                        _logger.Error("{PropertyName} had value {Value}, which was not of the expected type '{Type}", propertyName, propertyObj, typeof(T).FullName);
                        result.SetMessageDeadLettered(errorMessage);
                        await _messageReceiver.DeadLetterAsync(message.LockToken, errorMessage);
                    }
                }
            }

            return result;
        }
        public class GetItemResult<T>
        {
            public GetItemResult()
            {
                errorsList = new List<string>();
                WasMessageDeadLettered = false; // until set by SetMessageDeadLettered()
            }
            public T Item { get; set; }
            private IList<string> errorsList;
            public IEnumerable<string> Errors { get { return errorsList; } }
            public void SetMessageDeadLettered(string reason)
            {
                WasMessageDeadLettered = true;
                errorsList.Add(reason);
            }
            public bool WasMessageDeadLettered { get; private set; }
        }





        internal static ClaimsPrincipal CreatePrincipal(string claimsSerialized)
        {
            if (!string.IsNullOrWhiteSpace(claimsSerialized))
            {
                var claimValues = JsonConvert.DeserializeObject<IEnumerable<KeyValuePair<string, string>>>(claimsSerialized);
                var claims = claimValues.Select(x => new Claim(x.Key, x.Value));
                return new ClaimsPrincipal(new ClaimsIdentity(claims));
            }
            else
                return null;
        }




    }
}

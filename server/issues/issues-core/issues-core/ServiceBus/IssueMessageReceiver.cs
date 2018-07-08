using CoE.Ideas.Shared.Security;
using CoE.Ideas.Shared.ServiceBus;
using CoE.Issues.Core.Services;
using EnsureThat;
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
    //TODO: Mark messages as complete, abadoned or dead lettered!

    internal class IssueMessageReceiver : IIssueMessageReceiver
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
            if (_logger.IsEnabled(Serilog.Events.LogEventLevel.Information))
            {
                StringBuilder handlerNames = new StringBuilder();
                if (issueCreatedHandler != null)
                    handlerNames.Append("issueCreatedHandler");
                _logger.Information("Starting message pump with handlers " + handlerNames.ToString()); // + " on topic '{TopicName}' and subscription '{Subscription}'", _subscriptionClient.TopicPath, _subscriptionClient.SubscriptionName);
            }

            var messageHandlerOptions = options ?? new Microsoft.Azure.ServiceBus.MessageHandlerOptions(OnDefaultError);
            messageHandlerOptions.AutoComplete = false;
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

        protected virtual async Task ReceiveIssueCreated(Message msg, CancellationToken token, Func<IssueCreatedEventArgs, CancellationToken, Task> handler)
        {
            if (msg == null)
                throw new ArgumentNullException("msg");
            if (handler == null)
                throw new ArgumentNullException("handler");

            if (await EnsureMessageLabel(msg, IssueMessageSender.ISSUE_CREATED))
            {
                var id = await GetMessageString(msg, propertyName: nameof(IssueCreatedEventArgs.ReferenceId));
                if (id.WasMessageDeadLettered)
                    return;
                var remedyStatus = await GetMessageString(msg, propertyName: nameof(IssueCreatedEventArgs.RemedyStatus));
                if (remedyStatus.WasMessageDeadLettered)
                    return;
                var assigneeEmail = await GetMessageString(msg, propertyName: nameof(IssueCreatedEventArgs.AssigneeEmail), allowNullOrEmptyString: true);
                var requestorEmail = await GetMessageString(msg, propertyName: nameof(IssueCreatedEventArgs.RequestorEmail), allowNullOrEmptyString: true);
                var title = await GetMessageString(msg, propertyName: nameof(IssueCreatedEventArgs.Title), allowNullOrEmptyString: true);
                var description = await GetMessageString(msg, propertyName: nameof(IssueCreatedEventArgs.Description), allowNullOrEmptyString: true);

                try
                {
                    await handler(new IssueCreatedEventArgs()
                    {
                        Title = title.Item,
                        Description = description.Item,
                        AssigneeEmail = assigneeEmail.Item,
                        RequestorEmail = requestorEmail.Item,
                        RemedyStatus = remedyStatus.Item,
                        ReferenceId = id.Item
                    }, token);
                    await _messageReceiver.CompleteAsync(msg.LockToken);
                }
                catch (Exception err)
                {
                    _logger.Error(err, "IssueCreated handler threw the following error, abandoning message for future processing: {ErrorMessage}", err.Message);
                    await _messageReceiver.AbandonAsync(msg.LockToken);
                }
            }
        }

        protected virtual Task OnDefaultError(Microsoft.Azure.ServiceBus.ExceptionReceivedEventArgs err)
        {
            _logger.Error(err.Exception, "Error receiving message: {ErrorMessage}", err.Exception.Message);
            return Task.CompletedTask;
        }

        private async Task<bool> EnsureMessageLabel(Message message, string label)
        {
            if (message.Label == label)
                return true;
            else
            {
                await _messageReceiver.DeadLetterAsync(message.LockToken, $"Label was unexpected. Expected '{ label }', got '{ message.Label }';");
                return false;
            }
        }

        protected virtual async Task<GetItemResult<ClaimsPrincipal>> GetMessageOwner(Message message)
        {
            if (message == null)
                throw new ArgumentNullException("msg");

            var ownerClaimsResult = await GetMessageString(message, propertyName: "OwnerClaims");
            var result = new GetItemResult<ClaimsPrincipal>();
            if (ownerClaimsResult.WasMessageDeadLettered)
            {
                result.SetMessageDeadLettered(ownerClaimsResult.Errors.FirstOrDefault());
            }
            else
            {
                try
                {
                    result.Item = CreatePrincipal(ownerClaimsResult.Item);
                    _logger.Information("Message owner is {UserName} with email {Email}", result?.Item?.Identity?.Name, result?.Item?.GetEmail());
                }
                catch (Exception err)
                {
                    string errorMessage = $"Unable to get Owner from token { ownerClaimsResult.Item }: { err.Message }";
                    _logger.Error(err, "Unable to get Owner from token {Token}: {ErrorMessage}", ownerClaimsResult.Item, err.Message);
                    result.SetMessageDeadLettered(errorMessage);
                    await _messageReceiver.DeadLetterAsync(message.LockToken, errorMessage);
                }
            }

            // last fail safe
            if (!result.WasMessageDeadLettered && result.Item == null)
            {
                string errorMessage = $"Unable to get Owner, reason unknown";
                _logger.Error(errorMessage);
                result.SetMessageDeadLettered(errorMessage);
                await _messageReceiver.DeadLetterAsync(message.LockToken, errorMessage);
            }

            return result;
        }
        protected virtual async Task<GetItemResult<string>> GetMessageString(Message message, string propertyName, bool allowNullOrEmptyString = false)
        {
            if (message == null)
                throw new ArgumentNullException("msg");

            var result = await GetMessageProperty<string>(message, propertyName: propertyName, allowNull: allowNullOrEmptyString);
            if (!result.WasMessageDeadLettered && !allowNullOrEmptyString)
            {
                if (string.IsNullOrWhiteSpace(result.Item))
                {
                    string errorMessage = $"{ propertyName } was empty";
                    _logger.Error("{PropertyName} was empty in Service Bus message", propertyName);
                    result.SetMessageDeadLettered(errorMessage);
                    await _messageReceiver.DeadLetterAsync(message.LockToken, errorMessage);
                }
            }
            return result;
        }

        protected virtual async Task<GetItemResult<T>> GetMessageProperty<T>(Message message, string propertyName, bool allowNull = false)
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

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

                    case IssueMessageSender.New_ISSUE_CREATED:
                        {
                            if (newissueCreatedHandler != null)
                            {
                                _logger.Information("New_ISSUE_CREATED");
                            await ReceiveNewIssueCreated(msg, token, newissueCreatedHandler);
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
            var createdDate = await GetMessageProperty<DateTime>(msg, propertyName: nameof(IssueCreatedEventArgs.CreatedDate));
            args.CreatedDate = createdDate.Item;



            // call the handler registered for this event
            await issueCreatedHandler(args, token);
        }



        protected virtual async Task ReceiveNewIssueCreated(Ideas.Shared.ServiceBus.Message msg, CancellationToken token, Func<NewIssueCreatedEventArgs, CancellationToken, Task> newissuehandler)
        {
            if (msg == null)
                throw new ArgumentNullException("msg");
            if (newissuehandler == null)
                throw new ArgumentNullException("handler");

            if (await EnsureMessageLabel(msg, IssueMessageSender.New_ISSUE_CREATED))
            {
                _logger.Information("Received IssueCreated message. Getting message owner");

                var owner = await GetMessageOwner(msg);
                if (owner.WasMessageDeadLettered)
                    return;
                var idea = await GetMessageIssue(msg, owner.Item);
                if (idea.WasMessageDeadLettered)
                    return;

                try
                {
                    await newissuehandler(new NewIssueCreatedEventArgs()
                    {
                        Issue = idea.Item,
                        Owner = owner.Item
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



        protected virtual async Task ReceiveIssueWorkItemCreated(Ideas.Shared.ServiceBus.Message msg, CancellationToken token, Func<NewIssueCreatedEventArgs, CancellationToken, Task> handler)
        {
            if (msg == null)
                throw new ArgumentNullException("msg");
            if (handler == null)
                throw new ArgumentNullException("handler");

            if (await EnsureMessageLabel(msg, IssueMessageSender.New_ISSUE_CREATED))
            {
                var owner = await GetMessageOwner(msg);
                if (owner.WasMessageDeadLettered)
                    return;

                var idea = await GetMessageIssue(msg, owner.Item);
                if (idea.WasMessageDeadLettered)
                    return;
                var workOrderId = await GetMessageString(msg, propertyName: nameof(NewIssueCreatedEventArgs.IncidentId));
                if (workOrderId.WasMessageDeadLettered)
                    return;


                try
                {
                    await handler(new NewIssueCreatedEventArgs()
                    {
                        Issue = idea.Item,
                        Owner = owner.Item,
                        IncidentId = workOrderId.Item,
                    }, token);
                    await _messageReceiver.CompleteAsync(msg.LockToken);
                }
                catch (Exception err)
                {
                    _logger.Error(err, "IssueWorkItemCreated handler threw the following error, abandoning message for future processing: {ErrorMessage}", err.Message);
                    await _messageReceiver.AbandonAsync(msg.LockToken);
                }
            }
        }


        protected virtual async Task<GetItemResult<Issue>> GetMessageIssue(Ideas.Shared.ServiceBus.Message message, ClaimsPrincipal owner = null)
        {
            if (message == null)
                throw new ArgumentNullException("msg");

            var issueIdResult = await GetMessageProperty<int>(message, propertyName: "IssueId");
            var result = new GetItemResult<Issue>();
            if (issueIdResult.WasMessageDeadLettered)
            {
                result.SetMessageDeadLettered(issueIdResult.Errors.FirstOrDefault());
            }
            else
            {
                try
                {
                    _logger.Information("Message is for issue {IssueId}, retrieving issue...", issueIdResult.Item);

                    // if the remote repositoty factory is populated, we'll use that,
                    // otherwise we'll just use the default
                    var ideaRepository = GetIssueRepository(owner);

                    result.Item = await ideaRepository.GetIssueAsync(issueIdResult.Item);
                    _logger.Information("Retrieved issue {IssueId}, has title '{Title}'", issueIdResult.Item, result?.Item?.Title);
                }
                catch (Exception err)
                {
                    string errorMessage = $"Unable to get Issue { issueIdResult.Item }: { err.Message }";
                    _logger.Error(err, "Unable to get Issue {IssueId}: {ErrorMessage}", issueIdResult.Item, err.Message);
                    result.SetMessageDeadLettered(errorMessage);
                    await _messageReceiver.DeadLetterAsync(message.LockToken, errorMessage);

                }

            }

            return result;
        }

        protected virtual async Task<GetItemResult<ClaimsPrincipal>> GetMessageOwner(Ideas.Shared.ServiceBus.Message message)
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

        protected virtual async Task<GetItemResult<string>> GetMessageString(Ideas.Shared.ServiceBus.Message message, string propertyName, bool allowNullOrEmptyString = false)
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

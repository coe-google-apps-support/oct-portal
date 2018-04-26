using CoE.Ideas.Core.Data;
using CoE.Ideas.Core.Services;
using CoE.Ideas.Shared.Security;
using CoE.Ideas.Shared.ServiceBus;
using EnsureThat;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CoE.Ideas.Core.ServiceBus
{
    //TODO: Mark messages as complete, abadoned or dead lettered!

    internal abstract class InitiativeMessageReceiver : IInitiativeMessageReceiver
    { 
        public InitiativeMessageReceiver(IMessageReceiver messageReceiver,
            Serilog.ILogger logger) 
        {
            EnsureArg.IsNotNull(messageReceiver);
            EnsureArg.IsNotNull(logger);
            _messageReceiver = messageReceiver;
            _logger = logger;
        }
        private readonly IMessageReceiver _messageReceiver;
        private readonly Serilog.ILogger _logger;

        protected abstract IInitiativeRepository GetInitiativeRepository(ClaimsPrincipal owner);


        private IDictionary<string, ICollection<Func<Message, CancellationToken, Task>>> MessageMap = new Dictionary<string, ICollection<Func<Message, CancellationToken, Task>>>();

        public void ReceiveMessages(Func<InitiativeCreatedEventArgs, CancellationToken, Task> initiativeCreatedHandler = null,
            Func<WorkOrderCreatedEventArgs, CancellationToken, Task> workOrderCreatedHandler = null, 
            Func<WorkOrderUpdatedEventArgs, CancellationToken, Task> workOrderUpdatedHandler = null,
            Func<InitiativeLoggedEventArgs, CancellationToken, Task> initiativeLoggedHandler = null,
            Microsoft.Azure.ServiceBus.MessageHandlerOptions options = null)
        {
            if (_logger.IsEnabled(Serilog.Events.LogEventLevel.Information))
            {
                StringBuilder handlerNames = new StringBuilder();
                if (initiativeCreatedHandler != null)
                    handlerNames.Append("initiativeCreatedHndler");
                if (workOrderCreatedHandler != null)
                {
                    if (handlerNames.Length > 0)
                        handlerNames.Append(", ");
                    handlerNames.Append("workOrderCreatedHandler");
                }
                if (workOrderUpdatedHandler != null)
                {
                    if (handlerNames.Length > 0)
                        handlerNames.Append(", ");
                    handlerNames.Append("workOrderUpdatedHandler");
                }
                if (initiativeLoggedHandler != null)
                {
                    if (handlerNames.Length > 0)
                        handlerNames.Append(", ");
                    handlerNames.Append("initiativeLoggedHandler");
                }
                _logger.Information("Starting message pump with handlers " + handlerNames.ToString()); // + " on topic '{TopicName}' and subscription '{Subscription}'", _subscriptionClient.TopicPath, _subscriptionClient.SubscriptionName);
            }

            var messageHandlerOptions = options ?? new Microsoft.Azure.ServiceBus.MessageHandlerOptions(OnDefaultError);
            messageHandlerOptions.AutoComplete = false;
            _messageReceiver.RegisterMessageHandler(async (msg, token) =>
            {
                _logger.Information("Received service bus message {MessageId}: {Label}", msg.Id.ToString(), msg.Label);

                switch (msg.Label)
                {
                    case InitiativeMessageSender.INITIATIVE_CREATED:
                    {
                        if (initiativeCreatedHandler != null)
                            await ReceiveInitiativeCreated(msg, token, initiativeCreatedHandler);
                        else
                            await _messageReceiver.CompleteAsync(msg.LockToken);
                        break;
                    }
                    case InitiativeMessageSender.REMEDY_WORK_ITEM_CREATED:
                        if (workOrderCreatedHandler != null)
                            await ReceiveInitiativeWorkItemCreated(msg, token, workOrderCreatedHandler);
                        else
                            await _messageReceiver.CompleteAsync(msg.LockToken);
                        break;
                    case InitiativeMessageSender.WORK_ORDER_UPDATED:
                        if (workOrderUpdatedHandler != null)
                            await ReceiveWorkOrderUpdated(msg, token, workOrderUpdatedHandler);
                        else
                            await _messageReceiver.CompleteAsync(msg.LockToken);
                        break;
                    case InitiativeMessageSender.INITIATIVE_LOGGED:
                        if (initiativeLoggedHandler != null)
                            await ReceiveInitiativeLogged(msg, token, initiativeLoggedHandler);
                        else
                            await _messageReceiver.CompleteAsync(msg.LockToken);
                        break;
                    default:
                    {
                        await _messageReceiver.DeadLetterAsync(msg.LockToken, $"Unknown message type: { msg.Label }");
                        break;
                    }
                }
            }, messageHandlerOptions);
        }

        protected virtual Task OnDefaultError(Microsoft.Azure.ServiceBus.ExceptionReceivedEventArgs err)
        {
            _logger.Error(err.Exception, "Error receiving message: {ErrorMessage}", err.Exception.Message);
            return Task.CompletedTask;
        }

        protected virtual async Task ReceiveInitiativeCreated(Message msg, CancellationToken token, Func<InitiativeCreatedEventArgs, CancellationToken, Task> handler)
        {
            if (msg == null)
                throw new ArgumentNullException("msg");
            if (handler == null)
                throw new ArgumentNullException("handler");

            if (await EnsureMessageLabel(msg, InitiativeMessageSender.INITIATIVE_CREATED))
            {
                _logger.Information("Received InitiativeCreated message. Getting message owner");

                var owner = await GetMessageOwner(msg);
                if (owner.WasMessageDeadLettered)
                    return;
                var idea = await GetMessageInitiative(msg, owner.Item);
                if (idea.WasMessageDeadLettered)
                    return;

                try
                {
                    await handler(new InitiativeCreatedEventArgs()
                    {
                        Initiative = idea.Item,
                        Owner = owner.Item
                    }, token);
                    await _messageReceiver.CompleteAsync(msg.LockToken);
                }
                catch (Exception err)
                {
                    System.Diagnostics.Trace.TraceWarning($"InitiativeCreated handler threw the following error, abandoning message for future processing: { err.Message }");
                    await _messageReceiver.AbandonAsync(msg.LockToken);
                }
            }
        }

        protected virtual async Task ReceiveInitiativeWorkItemCreated(Message msg, CancellationToken token, Func<WorkOrderCreatedEventArgs, CancellationToken, Task> handler)
        {
            if (msg == null)
                throw new ArgumentNullException("msg");
            if (handler == null)
                throw new ArgumentNullException("handler");

            if (await EnsureMessageLabel(msg, InitiativeMessageSender.REMEDY_WORK_ITEM_CREATED))
            {
                var owner = await GetMessageOwner(msg);
                if (owner.WasMessageDeadLettered)
                    return;

                var idea = await GetMessageInitiative(msg, owner.Item);
                if (idea.WasMessageDeadLettered)
                    return;
                var workOrderId = await GetMessageString(msg, propertyName: nameof(WorkOrderCreatedEventArgs.WorkOrderId));
                if (workOrderId.WasMessageDeadLettered)
                    return;
                var eta = await GetMessageProperty<DateTime?>(msg, propertyName: nameof(WorkOrderCreatedEventArgs.EtaUtc));
                if (eta.WasMessageDeadLettered)
                    return;

                try
                {
                    await handler(new WorkOrderCreatedEventArgs()
                    {
                        Initiative = idea.Item,
                        Owner = owner.Item,
                        WorkOrderId = workOrderId.Item,
                        EtaUtc = eta.Item
                    }, token);
                    await _messageReceiver.CompleteAsync(msg.LockToken);
                }
                catch (Exception err)
                {
                    System.Diagnostics.Trace.TraceWarning($"InitiativeWorkItemCreated handler threw the following error, abandoning message for future processing: { err.Message }");
                    await _messageReceiver.AbandonAsync(msg.LockToken);
                }
            }
        }

        protected virtual async Task ReceiveWorkOrderUpdated(Message msg, CancellationToken token, Func<WorkOrderUpdatedEventArgs, CancellationToken, Task> handler)
        {
            if (msg == null)
                throw new ArgumentNullException("msg");
            if (handler == null)
                throw new ArgumentNullException("handler");

            if (await EnsureMessageLabel(msg, InitiativeMessageSender.WORK_ORDER_UPDATED))
            {
                var workOrderId = await GetMessageString(msg, propertyName: nameof(WorkOrderCreatedEventArgs.WorkOrderId));
                if (workOrderId.WasMessageDeadLettered)
                    return;
                var remedyStatus = await GetMessageString(msg, propertyName: nameof(WorkOrderUpdatedEventArgs.RemedyStatus));
                if (remedyStatus.WasMessageDeadLettered)
                    return;
                var updatedStatus = await GetMessageString(msg, propertyName: nameof(WorkOrderUpdatedEventArgs.UpdatedStatus));
                if (updatedStatus.WasMessageDeadLettered)
                    return;
                var updateTime = await GetMessageProperty<DateTime>(msg, propertyName: nameof(WorkOrderUpdatedEventArgs.UpdatedDateUtc)); // "WorkOrderUpdateTimeUtc");
                if (updateTime.WasMessageDeadLettered)
                    return;
                var assigneeEmail = await GetMessageString(msg, propertyName: nameof(WorkOrderUpdatedEventArgs.AssigneeEmail), allowNullOrEmptyString: true);
                var assigneeDisplayname = await GetMessageString(msg, propertyName: nameof(WorkOrderUpdatedEventArgs.AssigneeDisplayName), allowNullOrEmptyString: true);
                var eta = await GetMessageProperty<DateTime?>(msg, propertyName: nameof(WorkOrderUpdatedEventArgs.EtaUtc));

                try
                {
                    await handler(new WorkOrderUpdatedEventArgs()
                    {
                        RemedyStatus = remedyStatus.Item,
                        UpdatedStatus = updatedStatus.Item,
                        UpdatedDateUtc = updateTime.Item,
                        WorkOrderId = workOrderId.Item,
                        AssigneeEmail = assigneeEmail.Item,
                        AssigneeDisplayName = assigneeDisplayname.Item
                    }, token);
                    await _messageReceiver.CompleteAsync(msg.LockToken);
                }
                catch (Exception err)
                {
                    System.Diagnostics.Trace.TraceWarning($"WorkOrderUpdated handler threw the following error, abandoning message for future processing: { err.Message }");
                    await _messageReceiver.AbandonAsync(msg.LockToken);
                }
            }
        }

        protected virtual async Task ReceiveInitiativeLogged(Message msg, CancellationToken token, Func<InitiativeLoggedEventArgs, CancellationToken, Task> handler)
        {
            if (msg == null)
                throw new ArgumentNullException("msg");
            if (handler == null)
                throw new ArgumentNullException("handler");

            if (await EnsureMessageLabel(msg, InitiativeMessageSender.INITIATIVE_LOGGED))
            {
                var owner = await GetMessageOwner(msg);
                if (owner.WasMessageDeadLettered)
                    return;
                var idea = await GetMessageInitiative(msg, owner.Item);
                if (idea.WasMessageDeadLettered)
                    return;
                var rangeUpdated = await GetMessageString(msg, propertyName: "RangeUpdated");
                if (rangeUpdated.WasMessageDeadLettered)
                    return;

                try
                {
                    await handler(new InitiativeLoggedEventArgs()
                    {
                        Initiative = idea.Item,
                        Owner = owner.Item,
                        RangeUpdated = rangeUpdated.Item
                    }, token);
                    await _messageReceiver.CompleteAsync(msg.LockToken);
                }
                catch (Exception err)
                {
                    System.Diagnostics.Trace.TraceWarning($"InitiativeLogged handler threw the following error, abandoning message for future processing: { err.Message }");
                    await _messageReceiver.AbandonAsync(msg.LockToken);
                }
            }
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


        protected virtual async Task<GetItemResult<Initiative>> GetMessageInitiative(Message message, ClaimsPrincipal owner)
        {
            if (message == null)
                throw new ArgumentNullException("msg");

            var initiativeIdResult = await GetMessageProperty<int>(message, propertyName: "InitiativeId");
            var result = new GetItemResult<Initiative>();
            if (initiativeIdResult.WasMessageDeadLettered)
            {
                result.SetMessageDeadLettered(initiativeIdResult.Errors.FirstOrDefault());
            }
            else
            {
                try
                {
                    _logger.Information("Message is for initiative {InitiativeId}, retrieving initiative...", initiativeIdResult.Item);

                    // if the remote repositoty factory is populated, we'll use that,
                    // otherwise we'll just use the default
                    var ideaRepository = GetInitiativeRepository(owner);

                    result.Item = await ideaRepository.GetInitiativeAsync(initiativeIdResult.Item);
                    _logger.Information("Retrieved initiative {InitiativeId}, has title '{Title}'", initiativeIdResult.Item, result?.Item?.Title);
                }
                catch (Exception err)
                {
                    string errorMessage = $"Unable to get Initiative { initiativeIdResult.Item }: { err.Message }";
                    _logger.Error(err, "Unable to get Initiative {InitiativeId}: {ErrorMessage}", initiativeIdResult.Item, err.Message);
                    result.SetMessageDeadLettered(errorMessage);
                    await _messageReceiver.DeadLetterAsync(message.LockToken, errorMessage);

                }

            }

            return result;
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

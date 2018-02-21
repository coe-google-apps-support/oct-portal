using CoE.Ideas.Core.Security;
using CoE.Ideas.Core.WordPress;
using Microsoft.Azure.ServiceBus;
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

    internal class InitiativeMessageReceiver : IInitiativeMessageReceiver
    {
        public InitiativeMessageReceiver(IIdeaRepository repository,
            IWordPressClient wordPressClient,
            ISubscriptionClient subscriptionClient,
            IJwtTokenizer jwtTokenizer)
        {
            _repository = repository ?? throw new ArgumentNullException("repository");
            _wordPressClient = wordPressClient ?? throw new ArgumentNullException("wordPressClient");
            _subscriptionClient = subscriptionClient ?? throw new ArgumentNullException("subscriptionClient");
            _jwtTokenizer = jwtTokenizer ?? throw new ArgumentNullException("jwtTokenizer");
        }
        private readonly IIdeaRepository _repository;
        private readonly IWordPressClient _wordPressClient;
        private readonly ISubscriptionClient _subscriptionClient;
        private readonly IJwtTokenizer _jwtTokenizer;

        /// <summary>
        /// Closes the Client. Closes the connections opened by it.
        /// </summary>
        /// <returns></returns>
        public Task CloseAsync()
        {
            return _subscriptionClient.CloseAsync();
        }

        private async Task CheckPrerequisitions<TArgs>(Message msg, TArgs args, bool checker)
        {

            await _subscriptionClient.DeadLetterAsync(msg.SystemProperties.LockToken, $"Unable to get Initiative { msg.UserProperties["InitiativeId"] } or user for OwnerToken { msg.UserProperties["OwnerToken"] }");

        }
        public void ReceiveInitiativeCreated(Func<InitiativeCreatedEventArgs, CancellationToken, Task> handler, MessageHandlerOptions options)
        {
            _subscriptionClient.RegisterMessageHandler(async (msg, token) =>
            {
                var idea = await GetMessageInitiative(msg);
                if (idea.WasMessageDeadLettered)
                    return;
                var owner = await GetMessageOwner(msg);
                if (owner.WasMessageDeadLettered)
                    return;

                try
                {
                    await handler(new InitiativeCreatedEventArgs()
                    {
                        Initiative = idea.Item,
                        Owner = owner.Item
                    }, token);
                    await _subscriptionClient.CompleteAsync(msg.SystemProperties.LockToken);
                }
                catch (Exception err)
                {
                    System.Diagnostics.Trace.TraceWarning($"InitiativeCreated handler threw the following error, abandoning message for future processing: { err.Message }");
                    await _subscriptionClient.AbandonAsync(msg.SystemProperties.LockToken);
                }
            }, options);
        }


        public void ReceiveInitiativeWorkItemCreated(Func<WorkOrderCreatedEventArgs, CancellationToken, Task> handler, MessageHandlerOptions options)
        {
            _subscriptionClient.RegisterMessageHandler(async (msg, token) =>
            {
                var idea = await GetMessageInitiative(msg);
                if (idea.WasMessageDeadLettered)
                    return;
                var owner = await GetMessageOwner(msg);
                if (owner.WasMessageDeadLettered)
                    return;
                var workOrderId = await GetMessageString(msg, propertyName: "WorkOrderId");
                if (workOrderId.WasMessageDeadLettered)
                    return;

                try
                {
                    await handler(new WorkOrderCreatedEventArgs()
                    {
                        Initiative = idea.Item,
                        Owner = owner.Item,
                        WorkOrderId = workOrderId.Item
                    }, token);
                    await _subscriptionClient.CompleteAsync(msg.SystemProperties.LockToken);
                }
                catch (Exception err)
                {
                    System.Diagnostics.Trace.TraceWarning($"InitiativeWorkItemCreated handler threw the following error, abandoning message for future processing: { err.Message }");
                    await _subscriptionClient.AbandonAsync(msg.SystemProperties.LockToken);
                }
            }, options);
        }


        public void ReceiveWorkOrderUpdated(Func<WorkOrderUpdatedEventArgs, CancellationToken, Task> handler, MessageHandlerOptions options)
        {
            _subscriptionClient.RegisterMessageHandler(async (msg, token) =>
            {
                var updatedStatus = await GetMessageString(msg, propertyName: "WorkOrderStatus");
                if (updatedStatus.WasMessageDeadLettered)
                    return;
                var workOrderId = await GetMessageString(msg, propertyName: "WorkOrderId");
                if (workOrderId.WasMessageDeadLettered)
                    return;
                var updateTime = await GetMessageProperty<DateTime>(msg, propertyName: "WorkOrderUpdateTimeUtc");
                if (updateTime.WasMessageDeadLettered)
                    return;

                try
                {
                    await handler(new WorkOrderUpdatedEventArgs()
                    {
                        UpdatedStatus = updatedStatus.Item,
                        UpdatedDateUtc = updateTime.Item,
                        WorkOrderId = workOrderId.Item
                    }, token);
                    await _subscriptionClient.CompleteAsync(msg.SystemProperties.LockToken);
                }
                catch (Exception err)
                {
                    System.Diagnostics.Trace.TraceWarning($"WorkOrderUpdated handler threw the following error, abandoning message for future processing: { err.Message }");
                    await _subscriptionClient.AbandonAsync(msg.SystemProperties.LockToken);
                }
            }, options);
        }


        public void ReceiveInitiativeLogged(Func<InitiativeLoggedEventArgs, CancellationToken, Task> handler, MessageHandlerOptions options)
        {
            _subscriptionClient.RegisterMessageHandler(async (msg, token) =>
            {
                var idea = await GetMessageInitiative(msg);
                if (idea.WasMessageDeadLettered)
                    return;
                var owner = await GetMessageOwner(msg);
                if (owner.WasMessageDeadLettered)
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
                }
                catch (Exception err)
                {
                    System.Diagnostics.Trace.TraceWarning($"InitiativeLogged handler threw the following error, abandoning message for future processing: { err.Message }");
                    await _subscriptionClient.AbandonAsync(msg.SystemProperties.LockToken);
                }
            }, options);
        }


        protected virtual async Task<GetItemResult<Idea>> GetMessageInitiative(Message message)
        {
            if (message == null)
                throw new ArgumentNullException("msg");

            var initiativeIdResult = await GetMessageProperty<long>(message, propertyName: "OwnerToken");
            var result = new GetItemResult<Idea>();
            if (initiativeIdResult.WasMessageDeadLettered)
            {
                result.SetMessageDeadLettered(initiativeIdResult.Errors.FirstOrDefault());
            }
            else
            {
                try
                {
                    result.Item = await _repository.GetIdeaAsync(initiativeIdResult.Item);
                }
                catch (Exception err)
                {
                    string errorMessage = $"Unable to get Initiative { initiativeIdResult.Item }: { err.Message }";
                    result.SetMessageDeadLettered(errorMessage);
                    await _subscriptionClient.DeadLetterAsync(message.SystemProperties.LockToken, errorMessage);

                }

            }

            return result;
        }

        protected virtual async Task<GetItemResult<ClaimsPrincipal>> GetMessageOwner(Message message)
        {
            if (message == null)
                throw new ArgumentNullException("msg");

            var ownerTokenResult = await GetMessageString(message, propertyName: "OwnerToken");
            var result = new GetItemResult<ClaimsPrincipal>();
            if (ownerTokenResult.WasMessageDeadLettered)
            {
                result.SetMessageDeadLettered(ownerTokenResult.Errors.FirstOrDefault());
            }
            else
            {

                try
                {
                    result.Item = _jwtTokenizer.CreatePrincipal(ownerTokenResult.Item);
                }
                catch (Exception err)
                {
                    string errorMessage = $"Unable to get Owner from token { ownerTokenResult.Item }: { err.Message }";
                    result.SetMessageDeadLettered(errorMessage);
                    await _subscriptionClient.DeadLetterAsync(message.SystemProperties.LockToken, errorMessage);
                }
            }

            // last fail safe
            if (!result.WasMessageDeadLettered && result.Item == null)
            {
                string errorMessage = $"Unable to get Owner, reason unknown";
                result.SetMessageDeadLettered(errorMessage);
                await _subscriptionClient.DeadLetterAsync(message.SystemProperties.LockToken, errorMessage);
            }

            return result;
        }

        protected virtual async Task<GetItemResult<string>> GetMessageString(Message message, string propertyName)
        {
            if (message == null)
                throw new ArgumentNullException("msg");

            var result = await GetMessageProperty<string>(message, propertyName: propertyName);
            if (!result.WasMessageDeadLettered)
            {
                if (string.IsNullOrWhiteSpace(result.Item))
                {
                    string errorMessage = $"{ propertyName } was empty";
                    result.SetMessageDeadLettered(errorMessage);
                    await _subscriptionClient.DeadLetterAsync(message.SystemProperties.LockToken, errorMessage);
                }
            }
            return result;
        }

        protected virtual async Task<GetItemResult<T>> GetMessageProperty<T>(Message message, string propertyName) 
        {
            if (message == null)
                throw new ArgumentNullException("msg");

            var result = new GetItemResult<T>();
            T propertyValue = default(T);
            if (!message.UserProperties.ContainsKey(propertyName))
            {
                string errorMessage = $"{ propertyName } not found in message";
                result.SetMessageDeadLettered(errorMessage);
                await _subscriptionClient.DeadLetterAsync(message.SystemProperties.LockToken, errorMessage);
            }
            else
            {
                object propertyObj = message.UserProperties[propertyName];
                if (propertyObj == null)
                {
                    string errorMessage = $"{ propertyName } was null";
                    result.SetMessageDeadLettered(errorMessage);
                    await _subscriptionClient.DeadLetterAsync(message.SystemProperties.LockToken, errorMessage);
                }
                else
                {
                    try
                    {
                        propertyValue = (T)propertyObj;
                    }
                    catch (Exception)
                    {
                        string errorMessage = $"{ propertyName } was not of type { typeof(T).FullName }";
                        result.SetMessageDeadLettered(errorMessage);
                        await _subscriptionClient.DeadLetterAsync(message.SystemProperties.LockToken, errorMessage);
                    }
                }
            }

            return result;
        }

        internal class GetItemResult<T>
        {
            private readonly Message message;

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


    }
}

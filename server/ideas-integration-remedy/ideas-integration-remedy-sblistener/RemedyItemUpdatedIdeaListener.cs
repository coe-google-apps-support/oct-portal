using CoE.Ideas.Core;
using CoE.Ideas.Core.ServiceBus;
using CoE.Ideas.Core.WordPress;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CoE.Ideas.Remedy.SbListener
{
    public class RemedyItemUpdatedIdeaListener
    {
        public RemedyItemUpdatedIdeaListener(
            IIdeaRepository ideaRepository,
            ISubscriptionClient subscriptionClient)
        {
            _ideaRepository = ideaRepository ?? throw new ArgumentNullException("ideaRepository");
            _subscriptionClient = subscriptionClient ?? throw new ArgumentNullException("subscriptionClient");

            subscriptionClient.RegisterMessageHandler(OnMessageReceived, OnMessageError);
        }

        private readonly IIdeaRepository _ideaRepository;
        private readonly ISubscriptionClient _subscriptionClient;

        protected virtual async Task OnMessageReceived(Message message, CancellationToken token)
        {
            // TODO: determine a better way to filter the messages
            if (message.UserProperties.ContainsKey("WorkItemId"))
            {
                await ProcessWorkItemCreated(message, token);
            }
            else if (message.Label == "WorkItemChange" && message.UserProperties.ContainsKey("WorkItemStatus"))
            {
                await ProcessWorkItemUpdated(message, token);
            }

        }

        protected virtual async Task ProcessWorkItemCreated(Message message, CancellationToken token)
        {
            // is of type "RemedyTicketCreated"
            var data = Encoding.UTF8.GetString(message.Body);
            var msg = JsonConvert.DeserializeObject<IdeaMessage>(data);
            if (msg.Type != IdeaMessageType.WorkItemTicketCreated)
            {
                await _subscriptionClient.DeadLetterAsync(message.SystemProperties.LockToken, $"Invalid message with IdeaId={ msg.IdeaId }, header contained WorkItemId but was of { msg.Type }");
            }
            else
            {
                long ideadId = msg.IdeaId;
                await _ideaRepository.SetWorkItemTicketIdAsync(msg.IdeaId, message.UserProperties["WorkItemId"] as string);
                await _subscriptionClient.CompleteAsync(message.SystemProperties.LockToken);
            }
        }

        protected virtual async Task ProcessWorkItemUpdated(Message message, CancellationToken token)
        {
            string workItemId = message.UserProperties["WorkItemId"] as string;
            if (string.IsNullOrWhiteSpace(workItemId))
            {
                var idea = await _ideaRepository.GetIdeaByWorkItemIdAsync(workItemId);
                if (idea == null)
                {
                    await _subscriptionClient.DeadLetterAsync(message.SystemProperties.LockToken, $"Could not find initiative with work Item with id { workItemId }");
                }
                else
                {
                    
                }
            }

        }

        protected virtual Task OnMessageError(ExceptionReceivedEventArgs e)
        {
            Trace.TraceError($"Service Bus Error: {e.Exception.Message }");
            return Task.CompletedTask;
        }

    }
}

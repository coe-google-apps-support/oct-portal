using CoE.Ideas.Core;
using CoE.Ideas.Core.ServiceBus;
using CoE.Ideas.Core.WordPress;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RemedyServiceReference;
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
            IUpdatableIdeaRepository ideaRepository,
            ISubscriptionClient subscriptionClient,
            ILogger<RemedyItemUpdatedIdeaListener> logger)
        {
            _ideaRepository = ideaRepository ?? throw new ArgumentNullException("ideaRepository");
            _subscriptionClient = subscriptionClient ?? throw new ArgumentNullException("subscriptionClient");
            _logger = logger ?? throw new ArgumentNullException("logger");

            subscriptionClient.RegisterMessageHandler(OnMessageReceived, OnMessageError);
        }

        private readonly IUpdatableIdeaRepository _ideaRepository;
        private readonly ISubscriptionClient _subscriptionClient;
        private readonly ILogger<RemedyItemUpdatedIdeaListener> _logger;

        protected virtual async Task OnMessageReceived(Message message, CancellationToken token)
        {
            // TODO: determine a better way to filter the messages
            if (message.Label == "Remedy Work Item Created")
            {
                await ProcessWorkItemCreated(message, token);
            }
            else if (message.Label == "Remedy Work Item Changed")
            {
                await ProcessWorkItemUpdated(message, token);
            }
            else
            {
                await _subscriptionClient.CompleteAsync(message.SystemProperties.LockToken);
            }

        }

        protected virtual async Task ProcessWorkItemCreated(Message message, CancellationToken token)
        {
            //var returnMessage = new Message
            //{
            //    Label = "Remedy Work Item Created"
            //};
            //returnMessage.UserProperties["IdeaId"] = idea.Id;
            //returnMessage.UserProperties["WorkItemId"] = remedyTicketId;
            //await _topicClient.SendAsync(returnMessage);



            //_logger.LogInformation($"Remedy Work Item has been created with id ");
            long ideaId = 0;
            string workItemId = null;
            try
            { 
                ideaId = message.UserProperties.ContainsKey("IdeaId") ? (long)message.UserProperties["IdeaId"] : 0;
                workItemId = message.UserProperties.ContainsKey("WorkItemId") ? message.UserProperties["WorkItemId"] as string : null;

                _logger.LogInformation($"Remedy has created a work item for initiative { ideaId } with remedy id { workItemId }");

            }
            catch (Exception err)
            {
                _logger.LogError($"Error retrieveing IdeaId or WorkItemId from Remedy Work Item Created event: { err.Message }. Properties were { message.UserProperties }");
                await _subscriptionClient.DeadLetterAsync(message.SystemProperties.LockToken, $"Error retrieveing IdeaId or WorkItemId from Remedy Work Item Created event: { err.Message }");
                return;
            }

            try
            {

                if (ideaId > 0 && !string.IsNullOrWhiteSpace(workItemId))
                    await _ideaRepository.SetWorkItemTicketIdAsync(ideaId, workItemId);
            }
            catch (Exception err)
            {
                _logger.LogError($"Unable to set work item id '{ workItemId }' to initiative id { ideaId }. Will retry later. Error was: { err.Message }");
                await _subscriptionClient.AbandonAsync(message.SystemProperties.LockToken);
                return;
            }

            await _subscriptionClient.CompleteAsync(message.SystemProperties.LockToken);
        }



        protected virtual async Task ProcessWorkItemUpdated(Message message, CancellationToken token)
        {
            Exception error = null;
            Exception recoverableError = null;

            if (message.UserProperties.ContainsKey("WorkItemId"))
            {
                string workItemId = message.UserProperties["WorkItemId"] as string;
                if (string.IsNullOrWhiteSpace(workItemId))
                {
                    error = new Exception("Received Remedy message with an empty WorkItemId");
                    _logger.LogWarning(error.Message);
                }
                else
                {
                    _logger.LogDebug($"Received a Remedy change event for WorkItemId { workItemId }");

                    Idea idea = null;
                    try
                    {
                        idea = await _ideaRepository.GetIdeaByWorkItemIdAsync(workItemId);
                    }
                    catch (Exception err)
                    {
                        _logger.LogError($"Received WorkItem change notification from Remedy for item with Id { message.UserProperties["WorkItemId"] } but got the following error when looking it up in the Idea repository: { err.Message }");
                        idea = null;
                        error = err;
                    }

                    if (idea == null)
                        _logger.LogWarning($"Remedy message received for WorkItemId { workItemId } but could not find an associated initiative");
                    else
                    {
                        _logger.LogDebug($"Initiative for WorkItemId { workItemId } is has Initiative Id { idea.Id }");
                        OutputMapping1GetListValues workItem = null;
                        try
                        {
                            var data = Encoding.UTF8.GetString(message.Body);
                            workItem = JsonConvert.DeserializeObject<OutputMapping1GetListValues>(data);
                        }
                        catch (Exception err)
                        {
                            _logger.LogError($"Received Remedy message but unable to deserialize into OutputMapping1GetListValues object: { err.Message }");
                            error = err;
                        }
                        if (workItem != null)
                        {
                            try
                            {
                                await ProcessRemedyWorkItemChanged(workItemId, workItem, idea);
                            }
                            catch (Exception err)
                            {
                                _logger.LogWarning($"Unable to update Initiative status in repository. Will place message back on service bus and try again later. Error message was: { err.Message }");
                                recoverableError = err;
                            }
                        }
                    }
                }
            }

            if (error != null)
                await _subscriptionClient.DeadLetterAsync(message.SystemProperties.LockToken, $"Error processing Remedy Change event: { error.Message }");
            else if (recoverableError != null)
                await _subscriptionClient.AbandonAsync(message.SystemProperties.LockToken);
            else
                await _subscriptionClient.CompleteAsync(message.SystemProperties.LockToken);

        }

        private async Task<Idea> ProcessRemedyWorkItemChanged(string workItemId, OutputMapping1GetListValues workItem, Idea idea)
        {
            // here we have the business logic of translating Remedy statuses into our statuses
            var newIdeaStatus = GetInitiativeStatusForRemedyStatus(workItem.Status);
            if (newIdeaStatus.HasValue && newIdeaStatus.Value != idea.Status)
            {
                // we must update our database!
                _logger.LogInformation($"Updating status of initiative { idea.Id } from { idea.Status } to { newIdeaStatus } because Remedy was updated on { workItem.Last_Modified_Date }");
                return await _ideaRepository.SetWorkItemStatusAsync(idea.Id, newIdeaStatus.Value);
            }
            else
            {
                _logger.LogDebug($"Initative is already at status { newIdeaStatus }, so ignoring update to WorkItemId { workItemId }");
                return idea;
            }
        }

        protected virtual InitiativeStatus? GetInitiativeStatusForRemedyStatus(StatusType remedyStatusType)
        {
            // here we have the business logic of translating Remedy statuses into our statuses
            InitiativeStatus newIdeaStatus;
            switch (remedyStatusType)
            {
                case StatusType.Assigned:
                    newIdeaStatus = InitiativeStatus.Submit;
                    break;
                case StatusType.Cancelled:
                    newIdeaStatus = InitiativeStatus.Cancelled;
                    break;
                case StatusType.Completed:
                    newIdeaStatus = InitiativeStatus.Deliver;
                    break;
                case StatusType.InProgress:
                    newIdeaStatus = InitiativeStatus.Review;
                    break;
                case StatusType.Planning:
                    newIdeaStatus = InitiativeStatus.Collaborate;
                    break;
                case StatusType.Closed:
                case StatusType.Pending:
                case StatusType.Rejected:
                case StatusType.WaitingApproval:
                default:
                    return null; // no change
            }
            return newIdeaStatus;
        }

        protected virtual Task OnMessageError(ExceptionReceivedEventArgs e)
        {
            Trace.TraceError($"Service Bus Error: {e.Exception.Message }");
            return Task.CompletedTask;
        }

    }
}

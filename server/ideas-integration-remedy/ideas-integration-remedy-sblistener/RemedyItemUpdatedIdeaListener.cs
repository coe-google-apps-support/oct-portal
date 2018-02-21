using CoE.Ideas.Core;
using CoE.Ideas.Core.ServiceBus;
using CoE.Ideas.Core.WordPress;
using CoE.Ideas.Remedy.Watcher.RemedyServiceReference;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Serilog.Context;
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
            IInitiativeMessageReceiver initiativeMessageReceiver,
            Serilog.ILogger logger)
        {
            _ideaRepository = ideaRepository ?? throw new ArgumentNullException("ideaRepository");
            _initiativeMessageReceiver = initiativeMessageReceiver ?? throw new ArgumentNullException("initiativeMessageReceiver");
            _logger = logger ?? throw new ArgumentNullException("logger");

            initiativeMessageReceiver.ReceiveMessages(
                workOrderCreatedHandler: OnInitiativeWorkItemCreated,
                workOrderUpdatedHandler: OnWorkOrderUpdatedAsync);
        }

        private readonly IUpdatableIdeaRepository _ideaRepository;
        private readonly IInitiativeMessageReceiver _initiativeMessageReceiver;
        private readonly Serilog.ILogger _logger;

        protected virtual async Task OnInitiativeWorkItemCreated(WorkOrderCreatedEventArgs args, CancellationToken token)
        {
            if (args == null)
                throw new ArgumentNullException("args");
            if (args.Initiative == null)
                throw new ArgumentException("Initiative cannot be null");
            if (string.IsNullOrWhiteSpace(args.WorkOrderId))
                throw new ArgumentException("WorkOrderId cannot be empty");

            using (LogContext.PushProperty("InitiativeId", args.Initiative.Id))
            {
                _logger.Information("Remedy has created a work item for initiative { InitiativeId } with remedy id { WorkOrderId }", args.Initiative.Id, args.WorkOrderId);

                try
                {
                    await _ideaRepository.SetWorkItemTicketIdAsync(args.Initiative.Id, args.WorkOrderId);

                    // first status will be Submitted
                    await _ideaRepository.SetWorkItemStatusAsync(args.Initiative.Id, InitiativeStatus.Submit);
                }
                catch (Exception err)
                {
                    _logger.Error(err, "Unable to set work item id '{ WorkOrderId }' to initiative id { InitiativeId }. Will retry later. Error was: { ErrorMessage }", 
                        args.WorkOrderId, args.Initiative.Id, err.Message);
                    throw;
                }
            }
        }

        protected virtual async Task<Idea> OnWorkOrderUpdatedAsync(WorkOrderUpdatedEventArgs args, CancellationToken token)
        {
            if (args == null)
                throw new ArgumentNullException("args");
            if (string.IsNullOrWhiteSpace(args.WorkOrderId))
                throw new ArgumentException("WorkOrderId cannot be empty");
            if (string.IsNullOrWhiteSpace(args.UpdatedStatus))
                throw new ArgumentException("UpdatedStatus cannot be empty");

            using (LogContext.PushProperty("WorkOrderId", args.WorkOrderId))
            {
                Idea idea = await GetInitiativeByWorkOrderId(args.WorkOrderId);

                if (idea == null)
                    _logger.Warning($"Remedy message received for WorkItemId { args.WorkOrderId } but could not find an associated initiative", args.WorkOrderId);
                else
                {
                    using (LogContext.PushProperty("InitiativeId", idea.Id))
                    {
                        var workOrderStatus = Enum.Parse<StatusType>(args.UpdatedStatus);

                        await UpdateIdeaWithNewWorkOrderStatus(idea, workOrderStatus, args.UpdatedDateUtc);
                        await UpdateIdeaAssignee(idea, args.AssigneeEmail);
                    }
                }

                return idea;
            }
        }



        protected async Task<Idea> GetInitiativeByWorkOrderId(string workOrderId)
        {
            Idea idea = null;
            try
            {
                idea = await _ideaRepository.GetIdeaByWorkItemIdAsync(workOrderId);
            }
            catch (Exception err)
            {
                _logger.Error(err, "Received WorkItem change notification from Remedy for item with Id { WorkOrderId } but got the following error when looking it up in the Idea repository: { ErrorMessage }",
                    workOrderId, err.Message);
                idea = null;
            }
            return idea;
        }

        protected async Task UpdateIdeaWithNewWorkOrderStatus(Idea initiative, StatusType workOrderStatus, DateTime workOrderLastModifiedUtc)
        {
            // here we have the business logic of translating Remedy statuses into our statuses
            var newIdeaStatus = GetInitiativeStatusForRemedyStatus(workOrderStatus);
            if (newIdeaStatus.HasValue && newIdeaStatus.Value != initiative.Status)
            {
                // we must update our database!
                _logger.Information("Updating status of initiative { InitiativeId } from { FromInitiativeStatus } to { ToIdeaStatus } because Remedy was updated on { LastModifiedDateUtc }",
                    initiative.Id, initiative.Status, newIdeaStatus, workOrderLastModifiedUtc);
                await _ideaRepository.SetWorkItemStatusAsync(initiative.Id, newIdeaStatus.Value);
            }
            else
            {
                _logger.Debug("Initative is already at status { InitiativeStatus }, so ignoring update to WorkItemId { WorkOrderId }", initiative.Status);
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

        private async Task UpdateIdeaAssignee(Idea idea, string assigneeEmail)
        {
            Person assignee = null;
            if (!string.IsNullOrWhiteSpace(assigneeEmail))
            {
                assignee = await _ideaRepository.GetPersonByEmail(assigneeEmail);
            }

            await _ideaRepository.SetInitiativeAssignee(idea.Id, assignee);
        }
    }
}

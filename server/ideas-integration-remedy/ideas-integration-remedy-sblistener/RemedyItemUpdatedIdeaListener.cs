using CoE.Ideas.Core.Data;
using CoE.Ideas.Core.ServiceBus;
using CoE.Ideas.Core.Services;
using CoE.Ideas.Remedy.Watcher.RemedyServiceReference;
using Serilog.Context;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CoE.Ideas.Remedy.SbListener
{
    public class RemedyItemUpdatedIdeaListener
    {
        public RemedyItemUpdatedIdeaListener(
            IInitiativeRepository ideaRepository,
            IPersonRepository personRepository,
            IInitiativeMessageReceiver initiativeMessageReceiver,
            Serilog.ILogger logger)
        {
            _ideaRepository = ideaRepository ?? throw new ArgumentNullException("ideaRepository");
            _personRepository = personRepository ?? throw new ArgumentNullException("personRepository");
            _initiativeMessageReceiver = initiativeMessageReceiver ?? throw new ArgumentNullException("initiativeMessageReceiver");
            _logger = logger ?? throw new ArgumentNullException("logger");

            initiativeMessageReceiver.ReceiveMessages(
                workOrderCreatedHandler: OnInitiativeWorkItemCreated,
                workOrderUpdatedHandler: OnWorkOrderUpdatedAsync);
        }

        private readonly IInitiativeRepository _ideaRepository;
        private readonly IPersonRepository _personRepository;
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
                _logger.Information("Remedy has created a work item for initiative {InitiativeId} with remedy id {WorkOrderId}", args.Initiative.Id, args.WorkOrderId);

                try
                {
                    args.Initiative.SetWorkOrderId(args.WorkOrderId);
                    args.Initiative.UpdateStatus(InitiativeStatus.Submit);
                    await _ideaRepository.UpdateInitiativeAsync(args.Initiative);
                }
                catch (Exception err)
                {
                    _logger.Error(err, "Unable to set work item id '{WorkOrderId}' to initiative id {InitiativeId}. Will retry later. Error was: {ErrorMessage}", 
                        args.WorkOrderId, args.Initiative.Id, err.Message);
                    throw;
                }
            }
        }

        protected virtual async Task<Initiative> OnWorkOrderUpdatedAsync(WorkOrderUpdatedEventArgs args, CancellationToken token)
        {
            if (args == null)
                throw new ArgumentNullException("args");
            if (string.IsNullOrWhiteSpace(args.WorkOrderId))
                throw new ArgumentException("WorkOrderId cannot be empty");
            if (string.IsNullOrWhiteSpace(args.UpdatedStatus))
                throw new ArgumentException("UpdatedStatus cannot be empty");

            using (LogContext.PushProperty("WorkOrderId", args.WorkOrderId))
            {
                Initiative idea = await GetInitiativeByWorkOrderId(args.WorkOrderId);

                if (idea == null)
                    _logger.Warning("Remedy message received for WorkItemId {WorkOrderId} but could not find an associated initiative", args.WorkOrderId);
                else
                {
                    using (LogContext.PushProperty("InitiativeId", idea.Id))
                    {
                        var workOrderStatus = Enum.Parse<StatusType>(args.UpdatedStatus);

                        await UpdateIdeaAssignee(idea, args.AssigneeEmail, args.AssigneeDisplayName);
                        await UpdateIdeaWithNewWorkOrderStatus(idea, workOrderStatus, args.UpdatedDateUtc);
                    }
                }

                return idea;
            }
        }



        protected async Task<Initiative> GetInitiativeByWorkOrderId(string workOrderId)
        {
            Initiative idea = null;
            try
            {
                idea = await _ideaRepository.GetInitiativeByWorkOrderIdAsync(workOrderId);
            }
            catch (Exception err)
            {
                _logger.Error(err, "Received WorkItem change notification from Remedy for item with Id {WorkOrderId} but got the following error when looking it up in the Idea repository: {ErrorMessage}",
                    workOrderId, err.Message);
                idea = null;
            }
            return idea;
        }

        protected async Task UpdateIdeaWithNewWorkOrderStatus(Initiative initiative, StatusType workOrderStatus, DateTime workOrderLastModifiedUtc)
        {
            // here we have the business logic of translating Remedy statuses into our statuses
            var newIdeaStatus = GetInitiativeStatusForRemedyStatus(workOrderStatus);
            if (newIdeaStatus.HasValue && newIdeaStatus.Value != initiative.Status)
            {
                // we must update our database!
                _logger.Information("Updating status of initiative {InitiativeId} from {FromInitiativeStatus} to {ToIdeaStatus} because Remedy was updated on {LastModifiedDateUtc}",
                    initiative.Id, initiative.Status, newIdeaStatus, workOrderLastModifiedUtc);
                initiative.UpdateStatus(newIdeaStatus.Value);
                await _ideaRepository.UpdateInitiativeAsync(initiative);
            }
            else
            {
                _logger.Debug("Initative is already at status {InitiativeStatus}, so ignoring update to WorkItemId {WorkOrderId}", initiative.Status);
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

        private async Task UpdateIdeaAssignee(Initiative idea, string assigneeEmail, string assigneeDisplayName)
        {
            //Person assignee = null;
            int assigneeId = 0;
            if (!string.IsNullOrWhiteSpace(assigneeEmail))
            {
                assigneeId = await _personRepository.GetPersonIdByEmailAsync(assigneeEmail);

                // TODO: create the user if they don't exist?
            }

            idea.SetAssigneeId(assigneeId);

            await _ideaRepository.UpdateInitiativeAsync(idea);
        }
    }
}

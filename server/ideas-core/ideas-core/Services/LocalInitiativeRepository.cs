using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using CoE.Ideas.Core.Data;
using CoE.Ideas.Shared;
using CoE.Ideas.Shared.Security;
using EnsureThat;
using Microsoft.EntityFrameworkCore;

namespace CoE.Ideas.Core.Services
{
    internal class LocalInitiativeRepository : IInitiativeRepository
    {
        public LocalInitiativeRepository(InitiativeContext initiativeContext,
            Serilog.ILogger logger)
        {
            EnsureArg.IsNotNull(initiativeContext);
            EnsureArg.IsNotNull(logger);

            _initiativeContext = initiativeContext;
            _logger = logger;
        }

        private readonly InitiativeContext _initiativeContext;
        private readonly Serilog.ILogger _logger;


        public async Task<Initiative> AddInitiativeAsync(Initiative initiative)
        {
            EnsureArg.IsNotNull(initiative);

            _logger.Debug("Adding to Ideas database");
            _initiativeContext.Initiatives.Add(initiative);
            await _initiativeContext.SaveChangesAsync();

            return initiative;
        }

        public Task<Initiative> GetInitiativeAsync(Guid id)
        {
            return _initiativeContext.Initiatives.SingleOrDefaultAsync(x => x.Uid == id);
        }

        public Task<Initiative> GetInitiativeAsync(int id)
        {
            return _initiativeContext.Initiatives.FindAsync(id);
        }

        private static IQueryable<InitiativeInfo> CreateInitiativeInfoQuery(IQueryable<Initiative> query)
        {
            return query
                .Select(x => InitiativeInfo.Create(x));
        }

        public async Task<IEnumerable<InitiativeInfo>> GetInitiativesAsync()
        {
            //TODO: restrict to a reasonable amount of initiatives
            var initiatives = await CreateInitiativeInfoQuery(_initiativeContext.Initiatives)
                .ToListAsync();
            return initiatives;
        }

        public async Task<IEnumerable<InitiativeInfo>> GetInitiativesByStakeholderPersonIdAsync(int personId)
        {
            //TODO: restrict to a reasonable amount of initiatives
            var initiatives = await(CreateInitiativeInfoQuery(
                _initiativeContext.Initiatives
                    .Where(x => x.Stakeholders.Any(y => y.PersonId == personId))))
                .ToListAsync();

            return initiatives;
        }

        public async Task<Initiative> UpdateInitiativeAsync(Initiative initiative)
        {
            _initiativeContext.Entry(initiative).State = EntityState.Modified;
            await _initiativeContext.SaveChangesAsync();
            return initiative;
        }

        private struct InitiativeStepInfo
        {
            public InitiativeStep IdeaStep;
            public int StateId;
        }

        public async Task<IEnumerable<InitiativeStep>> GetInitiativeStepsAsync(int initiativeId)
        {
            var initiative = await _initiativeContext.Initiatives.FindAsync(initiativeId);
            if (initiative == null)
                throw new EntityNotFoundException($"Unable to find an initiative with id " + initiativeId);

            var statusHistories = await _initiativeContext.InitiativeStatusHistories
                //.Include(x => x.Assignee)
                .Where(x => x.InitiativeId == initiative.Uid)
                .OrderBy(x => x.StatusEntryDateUtc)
                .ToListAsync();

            var stack = new Stack<InitiativeStepInfo>();
            foreach (var sh in statusHistories)
            {

                while (stack.Count > 0 && stack.Peek().StateId > (int)sh.Status && sh.Status != InitiativeStatus.Cancelled)
                {
                    stack.Pop();
                }

                if (stack.Count > 0 && stack.Peek().StateId == (int)sh.Status)
                {
                    // nothing to do here
                    continue;
                }

                DateTimeOffset entryDate = new DateTime(sh.StatusEntryDateUtc.Ticks, DateTimeKind.Utc).ToLocalTime();
                DateTimeOffset? expectedExitDate = null;
                if (sh.ExpectedExitDateUtc.HasValue)
                    expectedExitDate = new DateTime(sh.ExpectedExitDateUtc.Value.Ticks, DateTimeKind.Utc).ToLocalTime();

                // update the finish date of the previous entry
                if (stack.Count > 0)
                    stack.Peek().IdeaStep.CompletionDate = entryDate;

                // add the new guy on top
                stack.Push(new InitiativeStepInfo()
                {
                    IdeaStep = new InitiativeStep()
                    {
                        AssigneePersonId = sh.PersonId,
                        Status = sh.Status,
                        StartDate = entryDate,
                        ExpectedCompletionDate = expectedExitDate
                    },
                    StateId = (int)sh.Status
                });
            }

            // finally, we need to make sure the current step does not have a completed date
            // (i.e. if something goes back a step it shouldn't be completed)
            if (stack.Any() && stack.Peek().IdeaStep.CompletionDate.HasValue)
            {
                stack.Peek().IdeaStep.CompletionDate = null;
            }

            var completedItems = stack.ToList();

            // now add the remaining ones
            var allStatuses = new InitiativeStatus[]
            {
                InitiativeStatus.Submit,
                InitiativeStatus.Review,
                InitiativeStatus.Collaborate,
                InitiativeStatus.Deliver
            };

            var remaining = allStatuses.Where(x => !stack.Any(y => y.StateId == (int)x)).OrderBy(x => (int)x)
                .Select(x => new InitiativeStepInfo() { IdeaStep = new InitiativeStep() { Status = x }, StateId = (int)x });
            return completedItems
                .Concat(remaining)
                .OrderBy(x => x.StateId)
                .Select(x => x.IdeaStep);
        }




        public Task<Initiative> GetInitiativeByWorkOrderIdAsync(string workOrderId)
        {
            return _initiativeContext.Initiatives
                .FirstOrDefaultAsync(x => x.WorkOrderId == workOrderId);
        }
    }
}

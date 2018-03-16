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

        public Task<Initiative> UpdateInitiativeAsync(Initiative initiative)
        {
            throw new NotImplementedException();
        }

        private struct InitiativeStepInfo
        {
            public InitiativeStep IdeaStep;
            public int stateId;
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

                while (stack.Count > 0 && stack.Peek().stateId > (int)sh.Status && sh.Status != InitiativeStatus.Cancelled)
                {
                    stack.Pop();
                }

                if (stack.Count > 0 && stack.Peek().stateId == (int)sh.Status)
                {
                    // nothing to do here
                    continue;
                }

                DateTimeOffset entryDate = new DateTime(sh.StatusEntryDateUtc.Ticks, DateTimeKind.Utc).ToLocalTime();

                // update the finish date of the previous entry
                if (stack.Count > 0)
                    stack.Peek().IdeaStep.CompletionDate = entryDate;

                // add the new guy on top
                stack.Push(new InitiativeStepInfo()
                {
                    IdeaStep = new InitiativeStep()
                    {
                        Title = GetInitiativeStepsAsync_GetTitle(sh.Status),
                        Description = sh.Text,
                        StartDate = entryDate
                    },
                    stateId = (int)sh.Status
                });
            }

            var completedItems = stack.Select(x => new { Step = x.IdeaStep, Order = x.stateId }).ToList();

            // now add the remaining ones
            var allStatuses = new InitiativeStatus[]
            {
                InitiativeStatus.Submit,
                InitiativeStatus.Review,
                InitiativeStatus.Collaborate,
                InitiativeStatus.Deliver
            };

            var remaining = allStatuses.Where(x => !stack.Any(y => y.stateId == (int)x)).OrderBy(x => (int)x);
            return completedItems
                .Concat(remaining.Select(x => new { Step = new InitiativeStep() { Title = GetInitiativeStepsAsync_GetTitle(x) }, Order = (int)x }))
                .OrderBy(x => x.Order)
                .Select(x => x.Step);
        }

        private string GetInitiativeStepsAsync_GetTitle(InitiativeStatus status)
        {
            switch (status)
            {
                case InitiativeStatus.Initiate:
                    return "Initiated";
                case InitiativeStatus.Submit:
                    return "Submitted";
                case InitiativeStatus.Review:
                    return "In Review";
                case InitiativeStatus.Collaborate:
                    return "In Collaboration";
                case InitiativeStatus.Deliver:
                    return "In Delivery";
                case InitiativeStatus.Cancelled:
                    return "Cancelled";
                default:
                    return status.ToString();
            }
        }


        public Task<Initiative> GetInitiativeByWorkOrderIdAsync(string workOrderId)
        {
            throw new NotImplementedException();
        }
    }
}

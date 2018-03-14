using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using CoE.Ideas.Core.Data;
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
            return _initiativeContext.Initiatives.FindAsync(id);
        }

        public Task<Initiative> GetInitiativeAsync(int id)
        {
            return _initiativeContext.Initiatives.FirstOrDefaultAsync(x => x.AlternateKey == id);
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

        public Task<IEnumerable<InitiativeStep>> GetInitiativeStepsAsync(int initiativeId)
        {
            throw new NotImplementedException();
        }

        public Task<Initiative> GetInitiativeByWorkOrderIdAsync(string workOrderId)
        {
            throw new NotImplementedException();
        }
    }
}

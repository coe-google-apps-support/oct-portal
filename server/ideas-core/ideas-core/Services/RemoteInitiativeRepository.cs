using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using CoE.Ideas.Core.Data;

namespace CoE.Ideas.Core.Services
{
    internal class RemoteInitiativeRepository : IInitiativeRepository
    {
        public void SetUser(ClaimsPrincipal user) { }

        public Task<Initiative> AddInitiativeAsync(Initiative initiative)
        {
            throw new NotImplementedException();
        }

        public Task<Initiative> GetInitiativeAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<Initiative> GetInitiativeAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<InitiativeInfo>> GetInitiativesAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<InitiativeInfo>> GetInitiativesByStakeholderPersonIdAsync(int personId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<InitiativeStep>> GetInitiativeStepsAsync(int initiativeId)
        {
            throw new NotImplementedException();
        }

        public Task<Initiative> UpdateInitiativeAsync(Initiative initiative)
        {
            throw new NotImplementedException();
        }

    }
}

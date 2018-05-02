using CoE.Ideas.Core.Data;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace CoE.Ideas.Core.Services
{
    public interface IInitiativeRepository
    {
        Task<Initiative> AddInitiativeAsync(Initiative initiative, CancellationToken cancellationToken = default(CancellationToken));
        Task<Initiative> UpdateInitiativeAsync(Initiative initiative);
        Task<Initiative> GetInitiativeAsync(Guid id);
        Task<Initiative> GetInitiativeAsync(int id);
        Task<Initiative> GetInitiativeByWorkOrderIdAsync(string workOrderId);
        Task<Initiative> GetInitiativeByApexId(int apexId);

        Task<IEnumerable<InitiativeInfo>> GetInitiativesAsync();
        Task<IEnumerable<InitiativeInfo>> GetInitiativesByStakeholderPersonIdAsync(int personId);

        //Task<IEnumerable<InitiativeStep>> GetInitiativeStepsAsync(int initiativeId);
    }
}

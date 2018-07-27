using CoE.Ideas.Core.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoE.Ideas.Core.Services
{
    public interface IInitiativeStatusEtaRepository
    {
        Task<IEnumerable<StatusEta>> GetStatusEtasAsync();
    }
}
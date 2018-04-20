using CoE.Ideas.Core.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CoE.Ideas.Core.Services
{
    public interface IInitiativeStatusEtaService
    {
        Task<DateTime?> GetStatusEtaFromNowUtcAsync(InitiativeStatus initiativeStatus);
    }
}

using CoE.Ideas.Core.Data;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CoE.Ideas.Core.Services
{
    public interface IInitiativeStatusEtaService
    {
        Task<DateTime?> GetStatusEtaFromNowUtcAsync(InitiativeStatus initiativeStatus);
        void Authenticate(ClaimsPrincipal user);
    }
}

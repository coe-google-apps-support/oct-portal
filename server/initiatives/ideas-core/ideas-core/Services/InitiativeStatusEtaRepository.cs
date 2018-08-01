using CoE.Ideas.Core.Data;
using EnsureThat;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoE.Ideas.Core.Services
{
    internal class InitiativeStatusEtaRepository : IInitiativeStatusEtaRepository
    {
        public InitiativeStatusEtaRepository(InitiativeContext initiativeContext)
        {
            EnsureArg.IsNotNull(initiativeContext);
            _initiativeContext = initiativeContext;
        }

        private readonly InitiativeContext _initiativeContext;

        public async Task<IEnumerable<StatusEta>> GetStatusEtasAsync()
        {
            var returnValue = await _initiativeContext.StatusEtas.ToArrayAsync();
            return returnValue;
        }
    }
}

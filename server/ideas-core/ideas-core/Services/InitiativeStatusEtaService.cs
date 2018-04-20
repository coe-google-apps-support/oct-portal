using CoE.Ideas.Core.Data;
using EnsureThat;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CoE.Ideas.Core.Services
{
    internal class InitiativeStatusEtaService : IInitiativeStatusEtaService
    {
        public InitiativeStatusEtaService(InitiativeContext initiativeContext,
            IBusinessCalendarService businessCalendarService)
        {
            EnsureArg.IsNotNull(initiativeContext);
            EnsureArg.IsNotNull(businessCalendarService);

            _initiativeContext = initiativeContext;
            _businessCalendarService = businessCalendarService;
        }

        private readonly InitiativeContext _initiativeContext;
        private readonly IBusinessCalendarService _businessCalendarService;

        public async Task<DateTime?> GetStatusEtaFromNowUtcAsync(InitiativeStatus initiativeStatus)
        {
            var etaDefinition = await _initiativeContext.StatusEtas
                .FirstOrDefaultAsync(x => x.Status == initiativeStatus);

            if (etaDefinition != null)
            {
                DateTime returnValue;
                if (etaDefinition.EtaType == EtaType.BusinessSeconds)
                    returnValue = await _businessCalendarService.AddBusinessTime(DateTime.Now, TimeSpan.FromSeconds(etaDefinition.Time));
                else
                    returnValue = await _businessCalendarService.AddBusinessDays(DateTime.Now, etaDefinition.Time);
                returnValue = returnValue.ToUniversalTime();
                return returnValue;
            }
            else
                return null;
        }
    }
}

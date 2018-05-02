using CoE.Ideas.Core.Data;
using EnsureThat;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CoE.Ideas.Core.Services
{
    internal class InitiativeStatusEtaService : IInitiativeStatusEtaService
    {
        public InitiativeStatusEtaService(IInitiativeStatusEtaRepository initiativeStatusEtaRepository,
            IBusinessCalendarService businessCalendarService,
            Serilog.ILogger logger)
        {
            EnsureArg.IsNotNull(initiativeStatusEtaRepository);
            EnsureArg.IsNotNull(businessCalendarService);
            EnsureArg.IsNotNull(logger);

            _initiativeStatusEtaRepository = initiativeStatusEtaRepository;
            _businessCalendarService = businessCalendarService;
            _logger = logger;
        }

        private readonly IInitiativeStatusEtaRepository _initiativeStatusEtaRepository;
        private readonly IBusinessCalendarService _businessCalendarService;
        private readonly Serilog.ILogger _logger;

        private static IDictionary<InitiativeStatus, StatusEta> _statusToEtaMap = null;
        protected IDictionary<InitiativeStatus, StatusEta> StatusToEtaMap
        {
            get
            {
                if (_statusToEtaMap == null)
                {
                    var valuesTask = _initiativeStatusEtaRepository.GetStatusEtasAsync();
                    valuesTask.Wait();
                    var values = valuesTask.Result;
                    // The data should be unique tby Status, but to be same we'll distinct() the results
                    _statusToEtaMap = values.GroupBy(x => x.Status)
                        .ToDictionary(x => x.Key, x => x.OrderByDescending(y => y.Id).First());
                }
                return _statusToEtaMap;
            }
        }

        public async Task<DateTime?> GetStatusEtaFromNowUtcAsync(InitiativeStatus initiativeStatus)
        {
            if (StatusToEtaMap.ContainsKey(initiativeStatus))
            {
                var etaDefinition = StatusToEtaMap[initiativeStatus];
                TimeZoneInfo albertaTimeZone;
                try { albertaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("America/Edmonton"); }
                catch (TimeZoneNotFoundException)
                {
                    _logger.Error("Unable to find Mountain Standard Time zone");
                    throw;
                }
                var nowAlberta = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, albertaTimeZone);
                DateTime returnValue;
                if (etaDefinition.EtaType == EtaType.BusinessSeconds)
                    returnValue = await _businessCalendarService.AddBusinessTime(nowAlberta, TimeSpan.FromSeconds(etaDefinition.Time));
                else
                    returnValue = await _businessCalendarService.AddBusinessDays(nowAlberta, etaDefinition.Time);
                returnValue = TimeZoneInfo.ConvertTimeToUtc(returnValue, albertaTimeZone);
                return returnValue;
            }
            else
                return null;
        }

        public void Authenticate(ClaimsPrincipal user)
        {
            var remoteService = _initiativeStatusEtaRepository as IRemoteRepository;

            if (remoteService != null)
            {
                remoteService.SetUser(user);
            }
        }
    }
}

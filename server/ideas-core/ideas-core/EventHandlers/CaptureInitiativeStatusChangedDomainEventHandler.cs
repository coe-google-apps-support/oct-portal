using CoE.Ideas.Core.Data;
using CoE.Ideas.Core.Events;
using CoE.Ideas.Core.Services;
using EnsureThat;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CoE.Ideas.Core.EventHandlers
{
    public class CaptureInitiativeStatusChangedDomainEventHandler : INotificationHandler<InitiativeStatusChangedDomainEvent>
    {
        public CaptureInitiativeStatusChangedDomainEventHandler(Serilog.ILogger logger,
            IPersonRepository personRepository,
            IServiceProvider serviceProvider)
            //IStringTemplateService stringTemplateService,
            //InitiativeContext initiativeContext)
        {
            // Events are raised through Mediatr which required public classes and public constructors.
            // IStringTemplateService, IStatusEtaService and InitiativeContext are internal classes so 
            // cannot be declared as parameters in the constructors. But we can ask for the 
            // serviceProvider and get those internal classes in the constructor itself.
            EnsureArg.IsNotNull(serviceProvider);
            IStringTemplateService stringTemplateService = serviceProvider.GetService(typeof(IStringTemplateService)) as IStringTemplateService;
            IBusinessCalendarService businessCalendarService = serviceProvider.GetService(typeof(IBusinessCalendarService)) as IBusinessCalendarService;
            InitiativeContext initiativeContext = serviceProvider.GetService(typeof(InitiativeContext)) as InitiativeContext;

            EnsureArg.IsNotNull(logger);
            EnsureArg.IsNotNull(personRepository);
            EnsureArg.IsNotNull(stringTemplateService);
            EnsureArg.IsNotNull(businessCalendarService);
            EnsureArg.IsNotNull(initiativeContext);

            _logger = logger;
            _personRepository = personRepository;
            _stringTemplateService = stringTemplateService;
            _businessCalendarService = businessCalendarService;

            _initiativeContext = initiativeContext;
        }
        private readonly Serilog.ILogger _logger;
        private readonly IPersonRepository _personRepository;
        private readonly InitiativeContext _initiativeContext;
        private readonly IStringTemplateService _stringTemplateService;
        private readonly IBusinessCalendarService _businessCalendarService;

        public async Task Handle(InitiativeStatusChangedDomainEvent notification, CancellationToken cancellationToken)
        {
            EnsureArg.IsNotNull(notification);
            EnsureArg.IsNotNull(notification.Initiative);

            var initiative = notification.Initiative;

            var assignee = initiative.AssigneeId.HasValue ? await _personRepository.GetPersonAsync(initiative.AssigneeId.Value) : null;

            var statusEta = await GetEta(initiative.Status);
            if (statusEta.HasValue)
                statusEta = statusEta.Value.ToUniversalTime();
            var statusChange = InitiativeStatusHistory.CreateInitiativeStatusChange(initiative.Uid,
                initiative.Status,
                DateTime.UtcNow, 
                statusEta,
                initiative.AssigneeId);

            // now we add the new status
            _initiativeContext.InitiativeStatusHistories.Add(statusChange);

            await _initiativeContext.SaveChangesAsync();
        }


        protected virtual async Task<DateTime?> GetEta(InitiativeStatus initiativeStatus)
        {
            var etaDefinition = await _initiativeContext.StatusEtas
                .FirstOrDefaultAsync(x => x.Status == initiativeStatus);

            if (etaDefinition != null)
            {
                if (etaDefinition.EtaType == EtaType.BusinessSeconds)
                    return await _businessCalendarService.AddBusinessTime(DateTime.Now, TimeSpan.FromSeconds(etaDefinition.Time));
                else
                    return await _businessCalendarService.AddBusinessDays(DateTime.Now, etaDefinition.Time);
            }
            else
                return null;
        }
    }
}

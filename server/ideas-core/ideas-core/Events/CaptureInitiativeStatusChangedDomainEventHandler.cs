using CoE.Ideas.Core.Data;
using CoE.Ideas.Core.Services;
using EnsureThat;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CoE.Ideas.Core.Events
{
    public class CaptureInitiativeStatusChangedDomainEventHandler : INotificationHandler<InitiativeStatusChangedDomainEvent>
    {
        public CaptureInitiativeStatusChangedDomainEventHandler(Serilog.ILogger logger,
            IPersonRepository personRepository,
            IServiceProvider serviceProvider)
            //IStringTemplateService stringTemplateService,
            //InitiativeContext initiativeContext)
        {
            EnsureArg.IsNotNull(serviceProvider);
            IStringTemplateService stringTemplateService = serviceProvider.GetService(typeof(IStringTemplateService)) as IStringTemplateService;
            InitiativeContext initiativeContext = serviceProvider.GetService(typeof(InitiativeContext)) as InitiativeContext;

            EnsureArg.IsNotNull(logger);
            EnsureArg.IsNotNull(personRepository);
            EnsureArg.IsNotNull(stringTemplateService);
            EnsureArg.IsNotNull(initiativeContext);

            _logger = logger;
            _personRepository = personRepository;
            _stringTemplateService = stringTemplateService;
            _initiativeContext = initiativeContext;
        }
        private readonly Serilog.ILogger _logger;
        private readonly IPersonRepository _personRepository;
        private readonly InitiativeContext _initiativeContext;
        private readonly IStringTemplateService _stringTemplateService;

        public async Task Handle(InitiativeStatusChangedDomainEvent notification, CancellationToken cancellationToken)
        {
            EnsureArg.IsNotNull(notification);
            EnsureArg.IsNotNull(notification.Initiative);

            var initiative = notification.Initiative;
            // get the previous status change, if any
            var previousChange = await _initiativeContext.InitiativeStatusHistories.Where(x => x.InitiativeId == initiative.Uid)
                .OrderByDescending(x => x.StatusEntryDateUtc)
                .FirstOrDefaultAsync();

            var assignee = initiative.AssigneeId.HasValue ? await _personRepository.GetPersonAsync(initiative.AssigneeId.Value) : null;

            // update the previous change message to past tense
            if (previousChange != null)
            {
                var previousStatusTemplate = await _stringTemplateService.GetStatusChangeTextAsync(initiative.Status, isPastTense: true);
                string previousStatusAssigneeName = string.IsNullOrWhiteSpace(assignee?.Name)
                    ? "A representative" : assignee.Name;
                var text = string.Format(previousStatusTemplate, previousStatusAssigneeName);

                previousChange.UpdateText(text);
            }

            var template = await _stringTemplateService.GetStatusChangeTextAsync(initiative.Status, isPastTense: false);
            string newText = string.Format(template, assignee?.Name);
            var statusChange = InitiativeStatusHistory.CreateInitiativeStatusChange(initiative.Uid,
                initiative.Status,
                DateTime.UtcNow,
                initiative.AssigneeId,
                newText);

            // now we add the new status
            _initiativeContext.InitiativeStatusHistories.Add(statusChange);

            await _initiativeContext.SaveChangesAsync();
        }
    }
}

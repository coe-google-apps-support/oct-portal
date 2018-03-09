using CoE.Ideas.Core.Data;
using CoE.Ideas.Core.Services;
using CoE.Ideas.Core.WordPress;
using EnsureThat;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CoE.Ideas.Core.Events
{
    internal class CaptureInitiativeStatusChangedDomainEventHandler : INotificationHandler<InitiativeStatusChangedDomainEvent>
    {
        public CaptureInitiativeStatusChangedDomainEventHandler(Serilog.ILogger logger,
            IWordPressClient wordPressClient,
            IStringTemplateService stringTemplateService,
            InitiativeContext initiativeContext)
        {
            EnsureArg.IsNotNull(logger);
            EnsureArg.IsNotNull(wordPressClient);
            EnsureArg.IsNotNull(stringTemplateService);
            EnsureArg.IsNotNull(initiativeContext);

            _logger = logger;
            _wordPressClient = wordPressClient;
            _stringTemplateService = stringTemplateService;
            _initiativeContext = initiativeContext;
        }
        private readonly Serilog.ILogger _logger;
        private readonly IWordPressClient _wordPressClient;
        private readonly InitiativeContext _initiativeContext;
        private readonly IStringTemplateService _stringTemplateService;

        public async Task Handle(InitiativeStatusChangedDomainEvent notification, CancellationToken cancellationToken)
        {
            EnsureArg.IsNotNull(notification);
            EnsureArg.IsNotNull(notification.Initiative);

            var initiative = notification.Initiative;
            // get the previous status change, if any
            var previousChange = await _initiativeContext.IdeaStatusHistories.Where(x => x.InitiativeId == initiative.Id)
                .OrderByDescending(x => x.StatusEntryDateUtc)
                .FirstOrDefaultAsync();

            var assignee = initiative.AssigneeId.HasValue ? await _wordPressClient.GetUserAsync(initiative.AssigneeId.Value) : null;

            // update the previous change message to past tense
            if (previousChange != null)
            {
                previousChange.UpdateText(await _stringTemplateService.GetStatusChangeTextAsync(initiative.Status, assignee, isPastTense: true));
            }



            InitiativeStatusHistory.CreateInitiativeStatusChange(initiative.Id,
                initiative.Status,
                DateTime.UtcNow,
                initiative.AssigneeId,
                await _stringTemplateService.GetStatusChangeTextAsync(initiative.Status, assignee));

            // now we add the new status
            var statusChange = new IdeaStatusHistoryInternal()
            {
                Initiative = idea,
                StatusEntryDateUtc = DateTime.UtcNow,
                Status = idea.Status,
                Assignee = idea.Assignee
            };

            statusChange.Text = await _stringTemplateService.GetStatusChangeTextAsync(idea.Status, idea.Assignee);
            _context.IdeaStatusHistories.Add(statusChange);

            await _context.SaveChangesAsync();

            return _mapper.Map<IdeaInternal, Idea>(idea);
        }
    }
}

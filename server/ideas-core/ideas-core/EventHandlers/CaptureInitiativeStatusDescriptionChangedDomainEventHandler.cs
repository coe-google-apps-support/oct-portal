using CoE.Ideas.Core.Data;
using CoE.Ideas.Core.Events;
using EnsureThat;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CoE.Ideas.Core.EventHandlers
{
    public class CaptureInitiativeStatusDescriptionChangedDomainEventHandler : INotificationHandler<InitiativeStatusDescriptionUpdatedDomainEvent>
    {
        public CaptureInitiativeStatusDescriptionChangedDomainEventHandler(Serilog.ILogger logger,
            IServiceProvider serviceProvider)
        {
            // InitiativeContext is internal is this project
            InitiativeContext initiativeContext = serviceProvider.GetService(typeof(InitiativeContext)) as InitiativeContext;
            EnsureArg.IsNotNull(logger);
            EnsureArg.IsNotNull(initiativeContext);

            _logger = logger;
            _initiativeContext = initiativeContext;
        }

        private readonly Serilog.ILogger _logger;
        private readonly InitiativeContext _initiativeContext;


        public async Task Handle(InitiativeStatusDescriptionUpdatedDomainEvent notification, CancellationToken cancellationToken)
        {
            EnsureArg.IsNotNull(notification);
            EnsureArg.IsNotNull(notification.Initiative);

            // get most recent status
            var statusHistory = await _initiativeContext.InitiativeStatusHistories
                .Where(x => x.InitiativeId == notification.Initiative.Uid)
                .OrderByDescending(x => x.StatusEntryDateUtc)
                .FirstOrDefaultAsync(cancellationToken);

            if (statusHistory == null)
            {
                _logger.Error("Could not find an InitiativeStatusHistory for initiative {InitativeId}", notification.Initiative.Id);
                throw new InvalidOperationException($"Could not find an InitiativeStatusHistory for initiative {notification.Initiative.Id}");
            }
            if (notification.Initiative.Status != statusHistory.Status)
            {
                string errorMessage = $"Current InitiativeStatusHistory does not equal initiative's status ('{statusHistory.Status}'<>'{notification.Initiative.Status}'";
                _logger.Error(errorMessage + " for initiative {InitiativeId}", notification.Initiative.Id);
                throw new InvalidOperationException(errorMessage);
            }


            if (string.IsNullOrWhiteSpace(notification.NewStatusDescription))
                statusHistory.ResetStatusDescriptionToDefault();
            else
                statusHistory.OverrideStatusDescription(notification.NewStatusDescription);

            await _initiativeContext.SaveChangesAsync(cancellationToken);
        }
    }
}

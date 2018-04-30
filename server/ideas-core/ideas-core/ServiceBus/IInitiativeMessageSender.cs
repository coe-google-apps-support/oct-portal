using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CoE.Ideas.Core.ServiceBus
{
    public interface IInitiativeMessageSender
    {
        Task SendInitiativeCreatedAsync(InitiativeCreatedEventArgs args);
        Task SendInitiativeWorkOrderCreatedAsync(WorkOrderCreatedEventArgs args);
        Task SendWorkOrderUpdatedAsync(WorkOrderUpdatedEventArgs args);

        Task SendInitiativeLoggedAsync(InitiativeLoggedEventArgs args);
        Task SendInitiativeStatusDescriptionChangedAsync(InitiativeStatusDescriptionChangedEventArgs initiativeCreatedEventArgs);
    }
}

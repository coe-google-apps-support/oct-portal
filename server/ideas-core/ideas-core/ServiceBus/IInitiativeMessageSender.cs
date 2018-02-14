using CoE.Ideas.Core.WordPress;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CoE.Ideas.Core.ServiceBus
{
    public interface IInitiativeMessageSender
    {
        Task SendInitiativeCreatedAsync(Idea initiative, ClaimsPrincipal ownerPrincipal);
        Task SendInitiativeWorkItemCreatedAsync(Idea initiative, ClaimsPrincipal ownerPrincipal, string workOrderId);
       
    }
}

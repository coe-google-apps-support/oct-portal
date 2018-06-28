using CoE.Ideas.Core.Data;
using System.Security.Claims;

namespace CoE.Ideas.Core.ServiceBus
{
    public class InitiativeCreatedEventArgs
    {
        public Initiative Initiative { get; set; }
        public ClaimsPrincipal Owner { get; set; }
        public bool SkipEmailNotification { get; set; }
    }
}

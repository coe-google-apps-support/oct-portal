using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace CoE.Ideas.Core.ServiceBus
{
    public class InitiativeCreatedEventArgs
    {
        public Idea Initiative { get; set; }
        public ClaimsPrincipal Owner { get; set; }
    }
}

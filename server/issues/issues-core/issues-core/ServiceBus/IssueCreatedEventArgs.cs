using CoE.Issues.Core.Data;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace CoE.Issues.Core.ServiceBus
{
    public class IssueCreatedEventArgs
    {
        public Issue Issue { get; set; }
        public ClaimsPrincipal Owner { get; set; }
    }
}

using CoE.Issues.Core.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Claims;


namespace CoE.Issues.Core.ServiceBus
{
    public class IncidentCreatedEventArgs
    {

        public Issue Issue { get; set; }
        public ClaimsPrincipal Owner { get; set; }
    }
}

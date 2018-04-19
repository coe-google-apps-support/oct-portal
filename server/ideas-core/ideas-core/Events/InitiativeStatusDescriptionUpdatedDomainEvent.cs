using CoE.Ideas.Core.Data;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoE.Ideas.Core.Events
{
    public class InitiativeStatusDescriptionUpdatedDomainEvent : INotification
    {
        public Initiative Initiative { get; private set; }
        public string NewStatusDescription { get; private set; }


        internal InitiativeStatusDescriptionUpdatedDomainEvent(Initiative initiative, string newStatusDescription)
        {
            Initiative = initiative;
            NewStatusDescription = newStatusDescription;
        }
    }
}

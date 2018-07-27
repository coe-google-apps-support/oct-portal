using CoE.Ideas.Core.Data;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoE.Ideas.Core.Events
{
    public class InitiativeStatusChangedDomainEvent : INotification
    {
        public Guid InitiativeId { get; private set; }
        public InitiativeStatus PreviousStatus { get; private set; }

        internal InitiativeStatusChangedDomainEvent(Guid initiativeId, InitiativeStatus previousInitiativeStatus)
        {
            InitiativeId = initiativeId;
            PreviousStatus = previousInitiativeStatus;
        }

    }
}

using CoE.Ideas.Core.Data;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoE.Ideas.Core.Events
{
    public class StakeholderAddedDomainEvent : INotification
    {
        public Guid InitiativeId { get; private set; }
        public int UserId { get; private set; }
        public StakeholderType StakeholderType { get; set; }

        internal StakeholderAddedDomainEvent(Guid initiativeId, int userId, StakeholderType stakeholderType)
        {
            InitiativeId = initiativeId;
            UserId = userId;
            StakeholderType = stakeholderType;
        }

    }
}

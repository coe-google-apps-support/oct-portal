using CoE.Ideas.Core.Data;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoE.Ideas.Core.Events
{
    // For invormation about DDD events see
    //https://docs.microsoft.com/en-us/dotnet/standard/microservices-architecture/microservice-ddd-cqrs-patterns/domain-events-design-implementation

    public class InitiativeCreatedDomainEvent : INotification
    {
        public Guid InitiativeId { get; private set; }
        public int OwnerPeronId { get; private set; }
        public bool SkipEmailNotification { get; private set; }

        internal InitiativeCreatedDomainEvent(Guid initiativeId, int ownerPersonId, bool skipEmailNotification)
        {
            InitiativeId = initiativeId;
            OwnerPeronId = ownerPersonId;
            SkipEmailNotification = skipEmailNotification;
        }
    }
}

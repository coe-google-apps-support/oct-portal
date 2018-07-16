using System;
using MediatR;

namespace CoE.Issues.Core.Event
{

    // For invormation about DDD events see
    //https://docs.microsoft.com/en-us/dotnet/standard/microservices-architecture/microservice-ddd-cqrs-patterns/domain-events-design-implementation

    public class IssueCreatedDomainEvent : INotification
    {
        public Guid IssueId { get; private set; }
        public int OwnerPeronId { get; private set; }

        internal IssueCreatedDomainEvent(Guid IssueId, int ownerPersonId)
        {
            IssueId = IssueId;
            OwnerPeronId = ownerPersonId;
        }
    }
}

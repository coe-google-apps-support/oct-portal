﻿using CoE.Ideas.Core.Data;
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
        public Initiative Initiative { get; private set; }

        internal InitiativeCreatedDomainEvent(Initiative initiative)
        {
            Initiative = initiative;
        }
    }
}

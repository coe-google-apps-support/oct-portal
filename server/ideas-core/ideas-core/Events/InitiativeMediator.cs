using CoE.Ideas.Core.Data;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CoE.Ideas.Core.Events
{
    internal class InitiativeMediator // : Mediator
    {
        public Task DispatchDomainEventsAsync(InitiativeContext context)
        {
            // TODO: Implement mediator pattern described by 
            // https://docs.microsoft.com/en-us/dotnet/standard/microservices-architecture/microservice-ddd-cqrs-patterns/domain-events-design-implementation
            throw new NotImplementedException();
        }
    }
}

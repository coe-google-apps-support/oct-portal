using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CoE.Ideas.Shared.Data
{
    public class EventDbContext : DbContext

    {
        protected EventDbContext(
            DbContextOptions options,
            Serilog.ILogger logger,
            DomainEvents domainEvents) : base(options)
        {
            _domainEvents = domainEvents;
            _logger = logger;
        }

        protected EventDbContext()
        {
        }



        private readonly DomainEvents _domainEvents;
        private readonly Serilog.ILogger _logger;


        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            // Dispatch Domain Events collection.
            // From https://docs.microsoft.com/en-us/dotnet/standard/microservices-architecture/microservice-ddd-cqrs-patterns/domain-events-design-implementation:
            // Choices:
            // A) Right BEFORE committing data (EF SaveChanges) into the DB. This makes
            // a single transaction including side effects from the domain event
            // handlers that are using the same DbContext with Scope lifetime
            // B) Right AFTER committing data (EF SaveChanges) into the DB. This makes
            // multiple transactions. You will need to handle eventual consistency and
            // compensatory actions in case of failures.        

            // see https://ardalis.com/using-mediatr-in-aspnet-core-apps for Mediatr examples

            var result = await base.SaveChangesAsync(cancellationToken);

            if (_domainEvents != null)
            {
                try
                {
                    await _domainEvents.DispatchDomainEventsAsync(this);
                }
                catch (Exception err)
                {
                    if (_logger == null)
                        throw;
                    else
                        _logger.Error(err, "Error dispatching domain events: {ErrorMessage}", err.Message);
                }
            }

            return result;
        }

    }
}

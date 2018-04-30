using CoE.Ideas.Core.Events;
using CoE.Ideas.Shared.Data;
using EnsureThat;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CoE.Ideas.Core.Data
{
    internal class InitiativeContext : DbContext
    {
        public InitiativeContext(
            DbContextOptions<InitiativeContext> options,
            Serilog.ILogger logger,
            DomainEvents domainEvents) : base(options)
        {
            _domainEvents = domainEvents;
            _logger = logger;
        }

        private readonly DomainEvents _domainEvents;
        private readonly Serilog.ILogger _logger;

        public DbSet<Initiative> Initiatives { get; set; }
        public DbSet<InitiativeStatusHistory> InitiativeStatusHistories { get; set; }
        public DbSet<StringTemplate> StringTemplates { get; set; }
        public DbSet<StatusEta> StatusEtas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Initiative>()
                .HasIndex(i => i.WorkOrderId)
                .IsUnique();


            // we need to specifically tell the model about value types:
            // https://technet.microsoft.com/en-us/mt842503.aspx

            //modelBuilder.Entity<Initiative>().OwnsOne(i => i.AuditRecord);
        }

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

using CoE.Ideas.Core.Events;
using EnsureThat;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CoE.Ideas.Core.Data
{
    internal class InitiativeContext : DbContext
    {
        public InitiativeContext(
            DbContextOptions<InitiativeContext> options,
            InitiativeMediator mediator) : base(options)
        {
            EnsureArg.IsNotNull(mediator);
            _initiativeMediator = mediator;
        }

        private readonly InitiativeMediator _initiativeMediator;

        public DbSet<Initiative> Initiatives { get; set; }
        public DbSet<InitiativeStatusHistory> IdeaStatusHistories { get; set; }
        public DbSet<StringTemplate> StringTemplates { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
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

            await _initiativeMediator.DispatchDomainEventsAsync(this);

            var result = await base.SaveChangesAsync(cancellationToken);
            return result;
        }
    }
}

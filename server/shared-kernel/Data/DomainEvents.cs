using EnsureThat;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CoE.Ideas.Shared.Data
{
    public class DomainEvents
    {
        private static IList<Type> _handlers;
        static DomainEvents()
        {

            _handlers = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(x => x.GetInterfaces().Any(y => y.IsGenericType &&
                                                       y.GetGenericTypeDefinition() == typeof(INotificationHandler<>)) &&
                            x.GetConstructors().Any())
                .ToList();
        }

        private readonly IServiceProvider _serviceProvider;

        public DomainEvents(IMediator mediator, IServiceProvider serviceProvider)
        {
            EnsureArg.IsNotNull(mediator);
            _mediator = mediator;
            _serviceProvider = serviceProvider;
        }

        private readonly IMediator _mediator;

        private ISet<INotification> handlingEvents = new HashSet<INotification>();

        internal async Task DispatchDomainEventsAsync(DbContext context)
        {
            var entities = context.ChangeTracker.Entries()
                .Where(x => x.Entity.GetType().GetInterfaces().Any(y => y == typeof(IContainsDomainEvents)))
                .Select(x => x.Entity)
                .Cast<IContainsDomainEvents>();

            var events = entities
                .SelectMany(x => x.DomainEvents)
                .Where(x => !handlingEvents.Contains(x))
                .ToList();

            // add to the "handlingEvents", so as to be sure not to raise events more than once
            handlingEvents.UnionWith(events);

            foreach (var domainEvent in events)
            {
                await _mediator.Publish(domainEvent);


            }

            // finally, we can mark all the events as completed
            handlingEvents.Clear();

            foreach (var entity in entities)
                entity.ClearDomainEvents();
        }
    }

}
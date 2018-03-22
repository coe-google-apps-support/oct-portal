using CoE.Ideas.Shared.Data;
using EnsureThat;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CoE.Ideas.Core.Events
{
    internal class DomainEvents
    {
        private static IList<Type> _handlers;
        static DomainEvents()
        {
            _handlers = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(x => x.GetInterfaces().Any(y => y.IsGenericType && y.GetGenericTypeDefinition() == typeof(INotificationHandler<>)))
                .ToList();
        }

        public DomainEvents(IMediator mediator)
        {
            EnsureArg.IsNotNull(mediator);
            _mediator = mediator;
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
                //foreach (Type handlerType in _handlers)
                //{
                    //bool canHandleEvent = handlerType.GetInterfaces()
                    //    .Any(x => x.IsGenericType
                    //    && x.GetGenericTypeDefinition() == typeof(INotificationHandler<>)
                    //    && x.GenericTypeArguments[0] == domainEvent.GetType());

                    //if (canHandleEvent)
                    //{
                    //    var handler = Activator.CreateInstance(handlerType);
                    //    var map = handlerType.GetInterfaceMap(handlerType);
                    //    var genericHandlerInterfaceType = typeof(INotificationHandler<>).MakeGenericType(domainEvent.GetType());
                    //    var index = Array.IndexOf(map.InterfaceMethods, genericHandlerInterfaceType.GetMethod("Handle"));
                    //    var m = map.TargetMethods[index];
                    //    m.Invoke(handler, )
                        await _mediator.Publish(domainEvent);
                    //}
                //}
            }

            // finally, we can mark all the events as completed
            handlingEvents.Clear();

            foreach (var entity in entities)
                entity.ClearDomainEvents();
        }
    }
}

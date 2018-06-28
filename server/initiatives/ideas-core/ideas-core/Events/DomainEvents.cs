using CoE.Ideas.Shared.Data;
using EnsureThat;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
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


                // Commented out below is the hard way of doing it without Mediatr.  I left it in in case we ever need
                // to do it that way again.

                //var handlerType = typeof(INotificationHandler<>).MakeGenericType(domainEvent.GetType());
                //var eventHandlers = _handlers
                //    .Select(x => new { HandlerType = x, Interface = x.GetInterfaces().FirstOrDefault(y => y == handlerType) })
                //    .Where(x => x.Interface != null)
                //    .Select(x => new { x.HandlerType, Method = x.Interface.GetMethods().FirstOrDefault(y => y.Name == "Handle" && y.GetParameters().Count() == 2) })
                //    .Where(x => x.Method != null)
                //    .ToList();


                //foreach (var h in eventHandlers)
                //{
                //    // Activate the eventHandler using the constructor with the most arguments,
                //    // filling the parameters using Dependency Injection
                //    var constructor = h.HandlerType.GetConstructors().OrderByDescending(x => x.GetParameters().Count()).First();
                //    var constructorParameters = new List<object>();
                //    foreach (var p in constructor.GetParameters())
                //    {
                //        try
                //        {
                //            var service = _serviceProvider.GetRequiredService(p.ParameterType);
                //            constructorParameters.Add(service);
                //        }
                //        catch(Exception err)
                //        {
                //            throw new InvalidOperationException($"Unable to resolve dependency for service of type {p.ParameterType.FullName}: {err.Message}", err);
                //        }
                //    }
                //    object handler;
                //    try
                //    {
                //        handler = constructor.Invoke(constructorParameters.ToArray());
                //    }
                //    catch (Exception err)
                //    {
                //        throw new InvalidOperationException($"Unable to create handler of type {h.HandlerType.FullName}: {err.Message}", err);
                //    }

                //    try
                //    {
                //        var task = h.Method.Invoke(handler, new object[] { domainEvent, new System.Threading.CancellationToken() }) as Task;
                //        await task;
                //    }
                //    catch (Exception err)
                //    {
                //        throw new InvalidOperationException($"Error running event handler {h.HandlerType.FullName}: {err.Message}", err);
                //    }
                //}
            }

            // finally, we can mark all the events as completed
            handlingEvents.Clear();

            foreach (var entity in entities)
                entity.ClearDomainEvents();
        }
    }
}

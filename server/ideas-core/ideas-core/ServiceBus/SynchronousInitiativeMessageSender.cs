using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CoE.Ideas.Core.ServiceBus
{
    public class SynchronousInitiativeMessageSender : IInitiativeMessageSender
    {
        public SynchronousInitiativeMessageSender(SynchronousInitiativeMessageReceiver messageReceiver)
        {
            _messageReceiver = messageReceiver ?? throw new ArgumentNullException("messageReceiver");

        }

        private readonly SynchronousInitiativeMessageReceiver _messageReceiver;

        public Task SendInitiativeCreatedAsync(InitiativeCreatedEventArgs args)
        {
            var tasks = new List<Task>();
            var cancellationToken = new System.Threading.CancellationToken();
            foreach (var h in _messageReceiver.CreatedHandlers)
                tasks.Add(h(args, cancellationToken));
            return Task.WhenAll(tasks);
        }

        public Task SendInitiativeLoggedAsync(InitiativeLoggedEventArgs args)
        {
            var tasks = new List<Task>();
            var cancellationToken = new System.Threading.CancellationToken();
            foreach (var h in _messageReceiver.InitiativeLoggedHandlers)
                tasks.Add(h(args, cancellationToken));
            return Task.WhenAll(tasks);
        }

        public Task SendInitiativeWorkOrderCreatedAsync(WorkOrderCreatedEventArgs args)
        {
            var tasks = new List<Task>();
            var cancellationToken = new System.Threading.CancellationToken();
            foreach (var h in _messageReceiver.WorkOrderCreatedHandlers)
                tasks.Add(h(args, cancellationToken));
            return Task.WhenAll(tasks);
        }

        public Task SendWorkOrderUpdatedAsync(WorkOrderUpdatedEventArgs args)
        {
            var tasks = new List<Task>();
            var cancellationToken = new System.Threading.CancellationToken();
            foreach (var h in _messageReceiver.WorkOrderUpdatedHandlers)
                tasks.Add(h(args, cancellationToken));
            return Task.WhenAll(tasks);
        }
    }
}

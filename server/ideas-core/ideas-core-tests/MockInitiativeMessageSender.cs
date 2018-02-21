using CoE.Ideas.Core.ServiceBus;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CoE.Ideas.Core.Tests
{
    internal class MockInitiativeMessageSender : IInitiativeMessageSender
    {
        public MockInitiativeMessageSender(MockInitiativeMessageReceiver receiver)
        {
            _receiver = receiver;
        }

        private readonly MockInitiativeMessageReceiver _receiver;


        public async Task SendInitiativeCreatedAsync(InitiativeCreatedEventArgs args)
        {
            foreach (var handler in _receiver.CreatedHandlers)
            {
                await handler(args, new System.Threading.CancellationToken());
            }
        }

        public async Task SendInitiativeWorkOrderCreatedAsync(WorkOrderCreatedEventArgs args)
        {
            foreach (var handler in _receiver.WorkOrderCreatedHandlers)
            {
                await handler(args, new System.Threading.CancellationToken());
            }
        }

        public async Task SendWorkOrderUpdatedAsync(WorkOrderUpdatedEventArgs args)
        {
            foreach (var handler in _receiver.WorkOrderUpdatedHandlers)
            {
                await handler(args, new System.Threading.CancellationToken());
            }
        }

        public async Task SendInitiativeLoggedAsync(InitiativeLoggedEventArgs args)
        {
            foreach (var handler in _receiver.InitiativeLoggedHandlers)
            {
                await handler(args, new System.Threading.CancellationToken());
            }
        }
    }

}

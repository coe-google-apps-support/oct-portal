using CoE.Ideas.Core;
using CoE.Ideas.Core.ServiceBus;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CoE.Ideas.EndToEnd.Tests
{
    internal class MockInitiativeMessageSender : IInitiativeMessageSender
    {
        public MockInitiativeMessageSender(MockInitiativeMessageReceiver receiver)
        {
            _receiver = receiver;
        }

        private readonly MockInitiativeMessageReceiver _receiver;

        public async Task SendInitiativeCreatedAsync(Idea initiative, ClaimsPrincipal ownerPrincipal)
        {
            foreach (var handler in _receiver.CreatedHandlers)
            {
                await handler(new InitiativeCreatedEventArgs()
                    {
                        Initiative = initiative,
                        Owner = ownerPrincipal
                    },
                    new System.Threading.CancellationToken());
            }
        }

        public async Task SendInitiativeWorkItemCreatedAsync(Idea initiative, ClaimsPrincipal ownerPrincipal, string workOrderId)
        {
            foreach (var handler in _receiver.WorkOrderCreatedHandlers)
            {
                await handler(new WorkOderCreatedEventArgs()
                    {
                        Initiative = initiative,
                        Owner = ownerPrincipal,
                        WorkOrderId = workOrderId
                    }, 
                    new System.Threading.CancellationToken());
            }
        }
    }
}

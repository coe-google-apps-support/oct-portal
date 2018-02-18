using System.Threading.Tasks;
using CoE.Ideas.Core.ServiceBus;

namespace CoE.Ideas.Remedy.Tests
{
    internal class MockInitiativeMessageSender : IInitiativeMessageSender
    {
        public Task SendInitiativeCreatedAsync(InitiativeCreatedEventArgs args)
        {
            // Not applicable to Remedy tests
            throw new System.NotImplementedException();
        }

        public Task SendInitiativeLoggedAsync(InitiativeLoggedEventArgs args)
        {
            // Not applicable to Remedy tests
            throw new System.NotImplementedException();
        }

        public Task SendInitiativeWorkOrderCreatedAsync(WorkOrderCreatedEventArgs args)
        {
            // do nothing
            return Task.CompletedTask;
        }

        public Task SendWorkOrderUpdatedAsync(WorkOrderUpdatedEventArgs args)
        {
            // do nothing
            return Task.CompletedTask;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CoE.Ideas.Shared.ServiceBus;
using EnsureThat;
using Microsoft.Azure.ServiceBus;

namespace CoE.Ideas.Core.ServiceBus
{
    internal class EmulatedServiceBusMessageReceiver : IMessageReceiver
    {
        public EmulatedServiceBusMessageReceiver(IServiceBusEmulator serviceBusEmulator)
        {
            EnsureArg.IsNotNull(serviceBusEmulator);

            _serviceBusEmulator = serviceBusEmulator;
        }
        private readonly IServiceBusEmulator _serviceBusEmulator;

        public Task AbandonAsync(string lockToken, IDictionary<string, object> propertiesToModify = null)
        {
            // nothing to do
            return Task.CompletedTask;
        }

        public Task CompleteAsync(string lockToken)
        {
            // nothing to do
            return Task.CompletedTask;
        }

        public Task DeadLetterAsync(string lockToken, string deadLetterReason, string deadLetterErrorDescription = null)
        {
            // nothing to do
            return Task.CompletedTask;
        }

        public void RegisterMessageHandler(Func<Shared.ServiceBus.Message, CancellationToken, Task> handler, MessageHandlerOptions messageHandlerOptions)
        {
            _serviceBusEmulator.CreateMessagePump(handler);
        }
    }
}

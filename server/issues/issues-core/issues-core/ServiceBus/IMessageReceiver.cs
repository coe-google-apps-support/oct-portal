using CoE.Ideas.Shared.ServiceBus;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CoE.Issues.Core.ServiceBus
{
    internal interface IMessageReceiver
    {
        Task AbandonAsync(string lockToken, IDictionary<string, object> propertiesToModify = null);
        Task CompleteAsync(string lockToken);
        Task DeadLetterAsync(string lockToken, string deadLetterReason, string deadLetterErrorDescription = null);
        void RegisterMessageHandler(Func<Message, CancellationToken, Task> handler, Microsoft.Azure.ServiceBus.MessageHandlerOptions messageHandlerOptions);
    }
}

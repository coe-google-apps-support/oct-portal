using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CoE.Ideas.Shared.ServiceBus
{
    public interface IServiceBusEmulator
    {
        Task PostAsync(IDictionary<string, object> properties = null, string label = null);
        void CreateMessagePump(Func<Message, CancellationToken, Task> onMessageReceived);
    }
}

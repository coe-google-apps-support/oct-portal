using System;
using System.Threading;
using System.Threading.Tasks;

namespace CoE.Issues.Core.ServiceBus
{
    internal interface IIssueMessgeReceiver
    {
        void ReceiveMessages(Func<IssueCreatedEventArgs, CancellationToken, Task> initiativeCreatedHandler = null,
            Microsoft.Azure.ServiceBus.MessageHandlerOptions options = null);
    }
}
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CoE.Issues.Core.ServiceBus
{
    internal interface IMessageSender
    {
        Task SendMessageAsync(string label, IDictionary<string, object> properties);
    }
}

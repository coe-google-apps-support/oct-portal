using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CoE.Issues.Core.ServiceBus
{
    public interface IMessageSender
    {
        Task SendMessageAsync(string label, object value);
    }
}

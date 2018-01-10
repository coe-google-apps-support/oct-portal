using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CoE.Ideas.Core.ServiceBus
{
    internal interface IQueueSender<T> where T : class
    {
        Task SendAsync(T item, Dictionary<string, object> properties);
    }
}

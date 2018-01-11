using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CoE.Ideas.Core.ServiceBus
{
    internal class NullTopicSender<T> : ITopicSender<T> where T : class
    {
        public NullTopicSender() { }

        public Task SendAsync(T item)
        {
            // Does nothing
            return Task.CompletedTask;
        }

        public Task SendAsync(T item, Dictionary<string, object> properties)
        {
            // Does nothing
            return Task.CompletedTask;
        }
    }
}

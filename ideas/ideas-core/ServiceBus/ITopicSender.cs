using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CoE.Ideas.Core.ServiceBus
{
    // based on https://github.com/TahirNaushad/Fiver.Azure.ServiceBus/blob/master/Fiver.Azure.ServiceBus/Topic/IAzureTopicSender.cs
    internal interface ITopicSender<T> where T : class
    {
        Task SendAsync(T item);
        Task SendAsync(T item, Dictionary<string, object> properties);
    }
}

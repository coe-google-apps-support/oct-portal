using EnsureThat;
using Microsoft.Azure.ServiceBus;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
namespace CoE.Issues.Core.ServiceBus
{
    internal class ServiceBusMessageSender : IMessageSender
    {
        public ServiceBusMessageSender(ITopicClient topicClient)
        {
            EnsureArg.IsNotNull(topicClient);
            _topicClient = topicClient;
        }
        private readonly ITopicClient _topicClient;

        public Task SendMessageAsync(string label)
        {
            return SendMessageAsync(label, null);
        }

        public Task SendMessageAsync(string label, IDictionary<string, object> properties)
        {
            var msg = new Message()
            {
                Label = label,
                MessageId = Guid.NewGuid().ToString()
            };
            foreach (var p in properties.Keys)
                msg.UserProperties[p] = properties[p];
            return _topicClient.SendAsync(msg);
        }

    }
}

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

        public Task SendMessageAsync(string label, object value)
        {
            var msg = new Message()
            {
                Label = label,
                MessageId = Guid.NewGuid().ToString()
            };
            if (value != null)
            {
                // one way to serialize the data
                var bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                using (var ms = new System.IO.MemoryStream())
                {
                    bf.Serialize(ms, value);
                    msg.Body = ms.ToArray();
                }
            }
            return _topicClient.SendAsync(msg);
        }

    }
}

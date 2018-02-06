using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CoE.Ideas.Core.ServiceBus
{
    // from https://www.codeproject.com/Articles/1204091/Azure-ServiceBus-in-NET-Core
    internal class QueueSender<T> : IQueueSender<T> where T : class
    {
        public QueueSender(IOptions<QueueSettings> options)
        {
            if (options == null)
                throw new ArgumentNullException("options");

            _settings = options.Value ?? throw new ArgumentNullException("settings");
            _client = new QueueClient(options.Value.ConnectionString, options.Value.QueueName);
        }

        private readonly QueueSettings _settings;
        private readonly QueueClient _client;

        public async Task SendAsync(T item, Dictionary<string, object> properties)
        {
            var json = JsonConvert.SerializeObject(item);
            var message = new Message(Encoding.UTF8.GetBytes(json));

            if (properties != null)
            {
                foreach (var prop in properties)
                {
                    message.UserProperties.Add(prop.Key, prop.Value);
                }
            }

            await _client.SendAsync(message);
        }
    }
}

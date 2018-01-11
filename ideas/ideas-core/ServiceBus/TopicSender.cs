using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CoE.Ideas.Core.ServiceBus
{
    // based on https://github.com/TahirNaushad/Fiver.Azure.ServiceBus/blob/master/Fiver.Azure.ServiceBus/Topic/AzureTopicSender.cs
    internal class TopicSender<T> : ITopicSender<T> where T : class
    {
        public TopicSender(IOptions<TopicSettings> options)
        {
            if (options == null)
                throw new ArgumentNullException("options");

            var settings = options.Value;
            if (settings == null)
                throw new ArgumentNullException("settings");

            _client = new TopicClient(settings.ConnectionString, settings.TopicName);
        }

        private readonly TopicClient _client;


        public async Task SendAsync(T item)
        {
            await SendAsync(item, null);
        }

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

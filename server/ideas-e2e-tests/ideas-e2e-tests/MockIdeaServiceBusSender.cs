using CoE.Ideas.Core;
using CoE.Ideas.Core.ServiceBus;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CoE.Ideas.EndToEnd.Tests
{
    internal class MockIdeaServiceBusSender : IIdeaServiceBusSender
    {
        public MockIdeaServiceBusSender(ITopicClient topicClient)
        {
            topicClient = _topicClient ?? throw new ArgumentNullException("topicClient");
        }
        private ITopicClient _topicClient;

        protected virtual void SetProperties(Idea idea, IdeaMessageType messageType, IDictionary<string, object> properties)
        {
            properties["AuthToken"] = "End2EndAuthToken_Fake";
            properties["IdeaMessageType"] = messageType.ToString();
        }


        public async Task SendIdeaMessageAsync(Idea idea, IdeaMessageType messageType)
        {
            var json = JsonConvert.SerializeObject(new IdeaMessage() { IdeaId = idea.Id, Type = messageType });
            var message = new Message(Encoding.UTF8.GetBytes(json));

            var props = new Dictionary<string, object>();
            SetProperties(idea, messageType, props);

            if (props != null)
            {
                foreach (var prop in props)
                {
                    message.UserProperties.Add(prop.Key, prop.Value);
                }
            }

            await _topicClient.SendAsync(message);
        }

        public async Task SendIdeaMessageAsync(Idea idea, IdeaMessageType messageType, Action<IDictionary<string, object>> onMessageHeaders)
        {
            var json = JsonConvert.SerializeObject(new IdeaMessage() { IdeaId = idea.Id, Type = messageType });
            var message = new Message(Encoding.UTF8.GetBytes(json));

            var props = new Dictionary<string, object>();
            SetProperties(idea, messageType, props);

            onMessageHeaders?.Invoke(props);

            try
            {
                await _topicClient.SendAsync(message);
            }
            catch (Exception err)
            {
                System.Diagnostics.Trace.TraceError($"Unable to send message to service bus: { err.Message }");
                throw;
            }
        }
    }
}

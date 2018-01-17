using CoE.Ideas.Core.Internal;
using CoE.Ideas.Core.ServiceBus;
using CoE.Ideas.Core.WordPress;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoE.Ideas.Core
{
    public class IdeaFactory
    {

        public static IIdeaRepository GetIdeaRepository(string uri)
        {
            return new RemoteIdeaRepository(uri);
        }

        public static IIdeaServiceBusReceiver GetServiceBusReceiver(
            string subscriptionName,
            string serviceBusConnectionString =null, 
            string topicName = null)
        {
            var options = new SimpleOptions<SubscriptionSettings>();
            options.Value = new SubscriptionSettings(serviceBusConnectionString, topicName, subscriptionName);
            var subscriptionReceiver = new SubscriptionReceiver<IdeaMessage>(options);
            return new IdeaServiceBusReceiver(subscriptionReceiver);
        }

        public static IIdeaServiceBusSender GetServiceBusSender(
            string serviceBusConnectionString,
            string topicName)
        {
            var options = new SimpleOptions<TopicSettings>();
            options.Value = new TopicSettings(serviceBusConnectionString, topicName);
            var topicSender = new TopicSender<IdeaMessage>(options);
            return new IdeaServiceBusSender(topicSender, null);
        }

        public static IWordPressClient GetWordPressClient(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentNullException("url");

            WordPressClientOptions options = new WordPressClientOptions();
            options.Url = new Uri(url);

            return new WordPressClient(new SimpleOptions<WordPressClientOptions>() { Value = options }, null);
        }

        private class SimpleOptions<T> : IOptions<T> where T : class, new()
        {
            public T Value { get; set; }
        }

    }
}

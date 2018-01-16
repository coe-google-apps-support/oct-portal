using System;

namespace CoE.Ideas.Core.ServiceBus
{
    // based on https://github.com/TahirNaushad/Fiver.Azure.ServiceBus/blob/master/Fiver.Azure.ServiceBus/Subscription/AzureSubscriptionSettings.cs
    internal class SubscriptionSettings
    {
        public SubscriptionSettings() { }

        public SubscriptionSettings(string connectionString, string topicName, string subscriptionName)
        {
            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentNullException("connectionString");

            if (string.IsNullOrEmpty(topicName))
                throw new ArgumentNullException("topicName");

            if (string.IsNullOrEmpty(subscriptionName))
                throw new ArgumentNullException("subscriptionName");

           ConnectionString = connectionString;
           TopicName = topicName;
           SubscriptionName = subscriptionName;
        }

        public string ConnectionString { get; set;  }
        public string TopicName { get; set;  }
        public string SubscriptionName { get; set;  }
    }
}
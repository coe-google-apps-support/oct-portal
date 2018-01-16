using System;
using System.Collections.Generic;
using System.Text;

namespace CoE.Ideas.Core.ServiceBus
{
    //based on https://github.com/TahirNaushad/Fiver.Azure.ServiceBus/blob/master/Fiver.Azure.ServiceBus/Topic/AzureTopicSettings.cs
    internal class TopicSettings
    {
        public TopicSettings() { }

        public TopicSettings(string connectionString, string topicName)
        {
            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentNullException("connectionString");

            if (string.IsNullOrEmpty(topicName))
                throw new ArgumentNullException("topicName");

            ConnectionString = connectionString;
            TopicName = topicName;
        }

        public string ConnectionString { get; set; }
        public string TopicName { get; set; }
    }
}

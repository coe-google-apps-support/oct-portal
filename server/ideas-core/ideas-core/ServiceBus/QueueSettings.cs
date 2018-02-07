using System;
using System.Collections.Generic;
using System.Text;

namespace CoE.Ideas.Core.ServiceBus
{
    public class QueueSettings
    {
        public QueueSettings() { }

        public QueueSettings(string connectionString, string queueName)
        {
            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentNullException("connectionString");

            if (string.IsNullOrEmpty(queueName))
                throw new ArgumentNullException("queueName");

            this.ConnectionString = connectionString;
            this.QueueName = queueName;
        }

        public string ConnectionString { get; set; }
        public string QueueName { get; set; }
    }
}

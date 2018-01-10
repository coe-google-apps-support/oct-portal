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
        public static IIdeaRepository GetIdeaRepository()
        {
            return new RemoteIdeaRepository("https://octportal.edmonton.ca:5000/api/ideas");
        }

        public static IIdeaRepository GetIdeaRepository(string uri)
        {
            return new RemoteIdeaRepository(uri);
        }

        public static QueueSettings GetIdeaQueue()
        {
            return new QueueSettings("TBD", "Ideas");
        }

        public static QueueSettings GetIdeaQueue(string connectionString)
        {
            return new QueueSettings(connectionString, "Ideas");
        }

        public static QueueSettings GetIdeaQueue(string connectionString, string queueName)
        {
            return new QueueSettings(connectionString, queueName);
        }

        public static IWordPressClient GetWordPressClient()
        {
            WordPressClientOptions options = new WordPressClientOptions();
            options.Url = new Uri("https://octportal.edmonton.ca");

            return new WordPressClient(new SimpleWordPressOptions() { Value = options }, null);
        }

        public static IWordPressClient GetWordPressClient(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentNullException("url");

            WordPressClientOptions options = new WordPressClientOptions();
            options.Url = new Uri("https://octportal.edmonton.ca");

            return new WordPressClient(new SimpleWordPressOptions() { Value = options }, null);
        }

        private class SimpleWordPressOptions : IOptions<WordPressClientOptions>
        {
            public WordPressClientOptions Value { get; set; }
        }


    }
}

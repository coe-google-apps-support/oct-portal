using System;
using System.IO;
using FluentAssertions;
using NUnit.Framework;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using System.Collections.Generic;
using CoE.Issues.Core.ServiceBus;

namespace CoE.Issues.Core.Tests
{
    class ServiceBus
    {
        private static ServiceProvider serviceProvider;

        [SetUp]
        public void Setup()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.test.json")
                .AddEnvironmentVariables()
                .Build();

            serviceProvider = new TestConfiguration(config)
                .ConfigureBasicServices()
                .AddServicebus()
                .BuildServiceProvider();
        }

        [Test]
        public void CanConnectTopicClient()
        {
            Assert.DoesNotThrow(() => 
            {
                serviceProvider.GetRequiredService<IMessageSender>();
            });
        }

        [Test]
        public void CanSendMessage()
        {
            IDictionary<string, object> testDict = new Dictionary<string, object>();
            testDict["one"] = "message 1";
            testDict["two"] = "message 2";
            testDict["three"] = "message 3";

            string message = "message";

            var messenger = serviceProvider.GetRequiredService<IMessageSender>();

            Func<Task> asyncFunction = async () =>
            {
                await messenger.SendMessageAsync(message, testDict);
                Console.WriteLine("Looks like it worked?");
            };
            asyncFunction.Should().NotThrow<Exception>();
        }
    }
}

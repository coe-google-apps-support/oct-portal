using System;
using System.IO;
using FluentAssertions;
using NUnit.Framework;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using System.Collections.Generic;
using CoE.Issues.Core.ServiceBus;
using CoE.Issues.Core.Data;
using System.Threading;

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
            };
            asyncFunction.Should().NotThrow<Exception>();
        }

        [Test]
        public void CanSendCreateIssueMessage()
        {
            var messenger = serviceProvider.GetRequiredService<IIssueMessageSender>();
            IssueCreatedEventArgs issueEvent = new IssueCreatedEventArgs()
            {
                Title = "A test issue",
                Description = "This is an issue created when running integration tests.",
                AssigneeEmail = "worker.bee@edmonton.ca",
                RequestorEmail = "requestor.bee@edmonton.ca",
                RemedyStatus = "Created",
                ReferenceId = "INC000123"
            };
            
            Func<Task> asyncFunction = async () =>
            {
                await messenger.SendIssueCreatedAsync(issueEvent);
            };
            asyncFunction.Should().NotThrow<Exception>();
        }

        [Test]
        public void CanReceiveCreateIssueMessage()
        {
            var autoEvent = new AutoResetEvent(false);
            var messenger = serviceProvider.GetRequiredService<IIssueMessageReceiver>();

            messenger.ReceiveMessages(issueCreatedHandler: (IssueCreatedEventArgs args, CancellationToken token) =>
            {
                var desc = args.Description;
                autoEvent.Set();
                return Task.FromResult<bool>(true);                
            });
            autoEvent.WaitOne();
        }
    }
}

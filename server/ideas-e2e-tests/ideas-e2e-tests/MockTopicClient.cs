using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CoE.Ideas.EndToEnd.Tests
{
    public class MockTopicClient : ITopicClient
    {
        //public MockTopicClient(Remedy.NewIdeaListener remedyNewIdeaListener)
        //{
        //    _remedyNewIdeaListener = remedyNewIdeaListener ?? throw new ArgumentNullException("remedyNewIdeaListener");
        //}

        private readonly Remedy.NewIdeaListener _remedyNewIdeaListener;

        public string TopicName => "End2End Tests - Initiatives";

        public Task SendAsync(Message message)
        {
            // route the messages appropriately
            

            return Task.CompletedTask;
        }

        public string ClientId => throw new NotImplementedException();

        public bool IsClosedOrClosing => throw new NotImplementedException();

        public TimeSpan OperationTimeout { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public IList<ServiceBusPlugin> RegisteredPlugins => throw new NotImplementedException();

        public Task CancelScheduledMessageAsync(long sequenceNumber)
        {
            throw new NotImplementedException();
        }

        public Task CloseAsync()
        {
            throw new NotImplementedException();
        }

        public void RegisterPlugin(ServiceBusPlugin serviceBusPlugin)
        {
            throw new NotImplementedException();
        }

        public Task<long> ScheduleMessageAsync(Message message, DateTimeOffset scheduleEnqueueTimeUtc)
        {
            throw new NotImplementedException();
        }


        public Task SendAsync(IList<Message> messageList)
        {
            throw new NotImplementedException();
        }

        public void UnregisterPlugin(string serviceBusPluginName)
        {
            throw new NotImplementedException();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CoE.Ideas.Core.ServiceBus
{
    internal class IdeaServiceBusReceiver : IIdeaServiceBusReceiver
    {
        public IdeaServiceBusReceiver(ISubscriptionReceiver<IdeaMessage> subscriptionReceiver)
        {
            _subscriptionReceiver = subscriptionReceiver ?? throw new ArgumentNullException("subscriptionReceiver");
        }

        private ISubscriptionReceiver<IdeaMessage> _subscriptionReceiver;

        public void ReceiveMessagesAsync(IIdeaListener listener)
        {
            _subscriptionReceiver.Receive(listener.OnMessageRecevied, listener.OnError, listener.OnWait);
        }
    }
}

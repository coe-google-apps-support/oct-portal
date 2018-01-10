using System;
using System.Collections.Generic;
using System.Text;

namespace CoE.Ideas.Core.ServiceBus
{
    public class IdeaQueue
    {
        public IdeaQueue(QueueSettings settings)
        {
            _settings = settings ?? throw new ArgumentNullException("settings");
        }
        private readonly QueueSettings _settings;

        public void RegisterIdeaListener(IIdeaListener listener)
        {
            new QueueReceiver<IdeaMessage>(_settings)
                .ReceiveMessages(listener.OnMessageRecevied, listener.OnError, listener.OnWait);
        }
    }
}

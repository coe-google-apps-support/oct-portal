using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CoE.Ideas.Core.ServiceBus
{
    public interface IIdeaServiceBusReceiver
    {
        void ReceiveMessagesAsync(IIdeaListener listener);

    }
}

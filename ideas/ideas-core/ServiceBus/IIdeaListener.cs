using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CoE.Ideas.Core.ServiceBus
{
    public interface IIdeaListener
    {
        Task<MessageProcessResponse> OnMessageRecevied(IdeaMessage message, IDictionary<string, object> properties);
        void OnError(Exception err);
        void OnWait();
    }
}

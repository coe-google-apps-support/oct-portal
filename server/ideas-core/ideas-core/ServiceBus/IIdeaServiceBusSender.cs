using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CoE.Ideas.Core.ServiceBus
{
    public interface IIdeaServiceBusSender
    {
        Task SendIdeaMessageAsync(Idea idea, IdeaMessageType messageType);
        Task SendIdeaMessageAsync(Idea idea, IdeaMessageType messageType, Action<IDictionary<string, object>> onMessageHeaders);

    }
}

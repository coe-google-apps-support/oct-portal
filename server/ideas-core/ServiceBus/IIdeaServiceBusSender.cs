using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CoE.Ideas.Core.ServiceBus
{
    public interface IIdeaServiceBusSender
    {
        Task SendIdeaCreatedMessageAsync(Idea idea);
        Task SendIdeaUpdatedMessageAsync(Idea idea);
    }
}

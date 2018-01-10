using System;
using System.Collections.Generic;
using System.Text;

namespace CoE.Ideas.Core.ServiceBus
{
    public class IdeaMessage
    {
        public long IdeaId { get; set; }
        public IdeaMessageType  Type { get; set; }
    }
}

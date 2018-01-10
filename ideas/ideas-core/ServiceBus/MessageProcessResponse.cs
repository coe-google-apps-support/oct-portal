using System;
using System.Collections.Generic;
using System.Text;

namespace CoE.Ideas.Core.ServiceBus
{
    public enum MessageProcessResponse
    {
        Complete,
        Abandon,
        Dead
    }
}

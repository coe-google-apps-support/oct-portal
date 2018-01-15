using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CoE.Ideas.Core.ServiceBus
{
    // based on https://github.com/TahirNaushad/Fiver.Azure.ServiceBus/blob/master/Fiver.Azure.ServiceBus/Subscription/IAzureSubscriptionReceiver.cs
    internal interface ISubscriptionReceiver<T> where T : class
    {
        void Receive(
            Func<T, IDictionary<string, object>, Task<MessageProcessResponse>> onProcess,
            Action<Exception> onError,
            Action onWait);
    }
}

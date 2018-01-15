using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CoE.Ideas.Core.ServiceBus
{
    // based on https://github.com/TahirNaushad/Fiver.Azure.ServiceBus/blob/master/Fiver.Azure.ServiceBus/Subscription/AzureSubscriptionReceiver.cs
    internal class SubscriptionReceiver<T> : ISubscriptionReceiver<T> where T : class
    {

        public SubscriptionReceiver(IOptions<SubscriptionSettings> options)
        {
            if (options == null)
                throw new ArgumentNullException("options");

            var settings = options.Value;
            if (settings == null)
                throw new ArgumentNullException("settings");

            _client = new SubscriptionClient(settings.ConnectionString, settings.TopicName, settings.SubscriptionName);
        }


        private readonly SubscriptionClient _client;

        public void Receive(
            Func<T, IDictionary<string, object>, Task<MessageProcessResponse>> onProcess,
            Action<Exception> onError,
            Action onWait)
        {
            var options = new MessageHandlerOptions(e =>
            {
                onError(e.Exception);
                return Task.CompletedTask;
            })
            {
                AutoComplete = false,
                MaxAutoRenewDuration = TimeSpan.FromMinutes(1)
            };

            _client.RegisterMessageHandler(
                async (message, token) =>
                {
                    try
                    {
                        // Get message
                        var data = Encoding.UTF8.GetString(message.Body);
                        T item = JsonConvert.DeserializeObject<T>(data);

                        // Process message
                        var result = await onProcess(item, message.UserProperties);

                        if (result == MessageProcessResponse.Complete)
                            await _client.CompleteAsync(message.SystemProperties.LockToken);
                        else if (result == MessageProcessResponse.Abandon)
                            await _client.AbandonAsync(message.SystemProperties.LockToken);
                        else if (result == MessageProcessResponse.Dead)
                            await _client.DeadLetterAsync(message.SystemProperties.LockToken);

                        // Wait for next message
                        onWait();
                    }
                    catch (Exception ex)
                    {
                        await _client.DeadLetterAsync(message.SystemProperties.LockToken);
                        onError(ex);
                    }
                }, options);
        }

    }
}

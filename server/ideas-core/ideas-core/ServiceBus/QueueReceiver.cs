using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CoE.Ideas.Core.ServiceBus
{
    // based on https://www.codeproject.com/Articles/1204091/Azure-ServiceBus-in-NET-Core
    internal class QueueReceiver<T> where T : class
    {
        public QueueReceiver(QueueSettings settings)
        {
            _settings = settings ?? throw new ArgumentNullException("settings");
            _client = new QueueClient(settings.ConnectionString, settings.QueueName);
        }

        private readonly QueueSettings _settings;
        private readonly QueueClient _client;

        public void ReceiveMessages(
            Func<T, Task<MessageProcessResponse>> onMessageReceived,
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
                        var result = await onMessageReceived(item);

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

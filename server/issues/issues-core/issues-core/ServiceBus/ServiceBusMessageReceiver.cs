using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EnsureThat;
using Microsoft.Azure.ServiceBus;
using Serilog;

namespace CoE.Issues.Core.ServiceBus
{
    internal class ServiceBusMessageReceiver : IMessageReceiver
    {
        public ServiceBusMessageReceiver(ISubscriptionClient subscriptionClient,
            ILogger logger)
        {
            EnsureArg.IsNotNull(subscriptionClient);
            EnsureArg.IsNotNull(logger);
            _subscriptionClient = subscriptionClient;
            _logger = logger;
        }
        private readonly ISubscriptionClient _subscriptionClient;
        private readonly ILogger _logger;


        public Task AbandonAsync(string lockToken, IDictionary<string, object> propertiesToModify = null)
        {
            return _subscriptionClient.AbandonAsync(lockToken, propertiesToModify);
        }

        public Task CompleteAsync(string lockToken)
        {
            return _subscriptionClient.CompleteAsync(lockToken);
        }


        public Task DeadLetterAsync(string lockToken, string deadLetterReason, string deadLetterErrorDescription = null)
        {
            return _subscriptionClient.DeadLetterAsync(lockToken, deadLetterReason, deadLetterErrorDescription);
        }

        public void RegisterMessageHandler(Func<Ideas.Shared.ServiceBus.Message, CancellationToken, Task> handler, MessageHandlerOptions messageHandlerOptions)
        {
            _subscriptionClient.RegisterMessageHandler(async (msg, token) =>
            {
                _logger.Debug("Received service bus message {MessageId}: {Label}", msg.MessageId, msg.Label);

                // transform msg to Message
                var messageDto = new Ideas.Shared.ServiceBus.Message()
                {
                    Id = Guid.Parse(msg.MessageId),
                    Label = msg.Label,
                    MessageProperties = msg.UserProperties,
                    CreatedDateUtc = msg.SystemProperties.EnqueuedTimeUtc,
                    LockToken = msg.SystemProperties.LockToken
                };
                if (msg.Body != null)
                {
                    // deserialize the same way it was serialized
                    using (var ms = new System.IO.MemoryStream())
                    {
                        var bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                        ms.Write(msg.Body, 0, msg.Body.Length);
                        ms.Seek(0, System.IO.SeekOrigin.Begin);
                        messageDto.Value = bf.Deserialize(ms);
                    }
                }
                await handler(messageDto, token);
            }, messageHandlerOptions);
        }

    }
}

using EnsureThat;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CoE.Ideas.Shared.ServiceBus
{
    internal class ServiceBusEmulator : IServiceBusEmulator
    {
        public ServiceBusEmulator(ServiceBusEmulatorContext context,
            ILogger logger)
        {
            EnsureArg.IsNotNull(context);
            EnsureArg.IsNotNull(logger);
            _context = context;
            _logger = logger;
        }

        private readonly ServiceBusEmulatorContext _context;
        private readonly ILogger _logger;

        public async Task PostAsync(IDictionary<string, object> properties = null, string label = null)
        {
            _logger.Information("Posting {Label} to service bus emulator", label);

            var messageEntity = new Message() { Id = Guid.NewGuid(), Label = label, CreatedDateUtc = DateTime.UtcNow, LockToken = Guid.NewGuid().ToString()};
            _context.Messages.Add(messageEntity);
            if (properties != null)
            {
                foreach (var propertyKey in properties.Keys)
                {
                    var propertyValue = properties[propertyKey];
                    string serializedValue, serializedValueType;
                    if (propertyValue == null)
                    {
                        serializedValue = null;
                        serializedValueType = "null";
                    }
                    else if (propertyValue is string)
                    {
                        serializedValue = (string)propertyValue;
                        serializedValueType = "string";
                    }
                    else
                    {
                        serializedValue = JsonConvert.SerializeObject(propertyValue);
                        serializedValueType = propertyValue.GetType().FullName;
                    }

                    _context.MessageProperties.Add(new MessageProperty()
                    {
                        Key = propertyKey,
                        Value = serializedValue,
                        ValueType = serializedValueType,
                        MessageId = messageEntity.Id
                    });
                }
            }
            await _context.SaveChangesAsync();
        }

        public void CreateMessagePump(Func<Message, CancellationToken, Task> onMessageReceived)
        {
            var t = new Thread(x =>
            {
                DateTime pollDate = DateTime.UtcNow;
                while (true)
                {
                    var nextDate = DateTime.UtcNow;
                    try
                    {

                        var messageProperties = _context.MessageProperties
                            .GroupBy(msgProp => msgProp.MessageId);

                        var items = _context.Messages
                            .Where(m => m.CreatedDateUtc > pollDate && m.CreatedDateUtc <= nextDate)
                            .Join(messageProperties, msg => msg.Id, msgProps => msgProps.Key, (msg, msgProps) => new { Message = msg, Properties = msgProps })
                            .OrderBy(m => m.Message.CreatedDateUtc)
                            .ToList();

                        foreach (var messageInfo in items)
                        {
                            // fixup for message properties
                            FixUpMessageProperties(messageInfo.Message, messageInfo.Properties);

                            try
                            {
                                var cancellationToken = new CancellationToken();
                                var task = onMessageReceived(messageInfo.Message, cancellationToken);
                                if (task != null)
                                {
                                    task.Wait(cancellationToken);
                                }
                            }
                            catch (Exception err)
                            {
                                // eat the message
                                _logger.Error(err, "ServiceBusEmulator onMessageReceived encountered the following exception upon receiving a message: {ErrorMessage}", err.Message);
                            }
                        }
                    }
                    finally
                    {
                        pollDate = nextDate;
                    }
                    Thread.Sleep(1000);
                }
            });
            t.Start();
        }

        private void FixUpMessageProperties(Message message, IEnumerable<MessageProperty> messageProperties)
        {
            var propDictionary = new Dictionary<string, object>();
            foreach (var mp in messageProperties)
            {
                object value;
                if (mp.ValueType == null || string.Equals(mp.ValueType, "null", StringComparison.OrdinalIgnoreCase))
                    value = null;
                else if (string.Equals(mp.ValueType, "string", StringComparison.OrdinalIgnoreCase))
                    value = mp.Value;
                else
                {
                    Type valueType = Type.GetType(mp.ValueType);
                    value = JsonConvert.DeserializeObject(mp.Value, valueType);
                }
                propDictionary[mp.Key] = value;
            }
            message.MessageProperties = propDictionary;

        }
    }
}

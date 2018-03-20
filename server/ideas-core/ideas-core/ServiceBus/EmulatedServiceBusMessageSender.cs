using CoE.Ideas.Shared.ServiceBus;
using EnsureThat;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CoE.Ideas.Core.ServiceBus
{
    internal class EmulatedServiceBusMessageSender : IMessageSender
    {
        public EmulatedServiceBusMessageSender(IServiceBusEmulator serviceBusEmulator)
        {
            EnsureArg.IsNotNull(serviceBusEmulator);

            _serviceBusEmulator = serviceBusEmulator;
        }
        private readonly IServiceBusEmulator _serviceBusEmulator;


        public Task SendMessageAsync(string label, IDictionary<string, object> properties)
        {
            return _serviceBusEmulator.PostAsync(properties: properties,
                label: label);
        }
    }
}

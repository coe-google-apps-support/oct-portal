using CoE.Ideas.Shared.ServiceBus;
using EnsureThat;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CoE.Ideas.Core.ServiceBus
{
    internal class ServiceBusEmulatedMessageSender : IInitiativeMessageSender
    {
        public ServiceBusEmulatedMessageSender(IServiceBusEmulator serviceBusEmulator)
        {
            EnsureArg.IsNotNull(serviceBusEmulator);

            _serviceBusEmulator = serviceBusEmulator;
        }

        private readonly IServiceBusEmulator _serviceBusEmulator;

        public Task SendInitiativeCreatedAsync(InitiativeCreatedEventArgs args)
        {
            var messageProperties = new Dictionary<string, object>();
            InitiativeMessageSender.SetInitiative(args.Initiative, messageProperties);
            InitiativeMessageSender.SetOwner(args.Owner, messageProperties);

            return _serviceBusEmulator.PostAsync(JsonConvert.SerializeObject(args), 
                properties: messageProperties, 
                label: InitiativeMessageSender.INITIATIVE_CREATED);
        }

        public Task SendInitiativeLoggedAsync(InitiativeLoggedEventArgs args)
        {
            return _serviceBusEmulator.PostAsync(JsonConvert.SerializeObject(args), label: InitiativeMessageSender.INITIATIVE_LOGGED);
        }

        public Task SendInitiativeWorkOrderCreatedAsync(WorkOrderCreatedEventArgs args)
        {
            return _serviceBusEmulator.PostAsync(JsonConvert.SerializeObject(args), label: InitiativeMessageSender.REMEDY_WORK_ITEM_CREATED);
        }

        public Task SendWorkOrderUpdatedAsync(WorkOrderUpdatedEventArgs args)
        {
            return _serviceBusEmulator.PostAsync(JsonConvert.SerializeObject(args), label: InitiativeMessageSender.WORK_ORDER_UPDATED);
        }
    }
}

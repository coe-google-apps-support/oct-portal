using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CoE.Ideas.Core.Services;
using CoE.Ideas.Shared.Security;
using CoE.Ideas.Shared.ServiceBus;
using EnsureThat;
using Newtonsoft.Json;
using Serilog;
using static CoE.Ideas.Core.ServiceBus.InitiativeMessageReceiver;

namespace CoE.Ideas.Core.ServiceBus
{
    internal class ServiceBusEmulatedMessageReceiver : IInitiativeMessageReceiver
    {
        public ServiceBusEmulatedMessageReceiver(IServiceBusEmulator serviceBusEmulator,
            IInitiativeRepository initiativeRepository,
            ILogger logger)
        {
            EnsureArg.IsNotNull(serviceBusEmulator);
            EnsureArg.IsNotNull(initiativeRepository);
            EnsureArg.IsNotNull(logger);

            _serviceBusEmulator = serviceBusEmulator;
            _initiativeRepository = initiativeRepository;
            _logger = logger;
        }

        private readonly IServiceBusEmulator _serviceBusEmulator;
        private readonly IInitiativeRepository _initiativeRepository;
        private readonly ILogger _logger;

        public void ReceiveMessages(Func<InitiativeCreatedEventArgs, CancellationToken, Task> initiativeCreatedHandler = null, 
            Func<WorkOrderCreatedEventArgs, CancellationToken, Task> workOrderCreatedHandler = null, 
            Func<WorkOrderUpdatedEventArgs, CancellationToken, Task> workOrderUpdatedHandler = null, 
            Func<InitiativeLoggedEventArgs, CancellationToken, Task> initiativeLoggedHandler = null,
            Microsoft.Azure.ServiceBus.MessageHandlerOptions options = null)
        {
            _serviceBusEmulator.CreateMessagePump(async (message, token) =>
            {
                throw new NotImplementedException();
                switch (message.Label)
                {

                    case InitiativeMessageSender.INITIATIVE_CREATED:
                        {
                            await ReceiveInitiativeCreated(message, token, initiativeCreatedHandler);
                            //await initiativeCreatedHandler?.Invoke(
                            //    JsonConvert.DeserializeObject<InitiativeCreatedEventArgs>(message.Text), 
                            //    token);
                            break;
                        }
                    case InitiativeMessageSender.REMEDY_WORK_ITEM_CREATED:
                        await workOrderCreatedHandler?.Invoke(
                            JsonConvert.DeserializeObject<WorkOrderCreatedEventArgs>(message.Text),
                            token);
                        break;
                    case InitiativeMessageSender.WORK_ORDER_UPDATED:
                        await workOrderUpdatedHandler?.Invoke(
                            JsonConvert.DeserializeObject<WorkOrderUpdatedEventArgs>(message.Text),
                            token);
                        break;
                    case InitiativeMessageSender.INITIATIVE_LOGGED:
                        await initiativeLoggedHandler?.Invoke(
                            JsonConvert.DeserializeObject<InitiativeLoggedEventArgs>(message.Text),
                            token);
                        break;
                    default:
                        {
                            _logger.Warning("Unknown Message type: {Label}", message.Label);
                            break;
                        }
                }
            });
        }

        protected internal virtual async Task ReceiveInitiativeCreated(Message msg, CancellationToken token, Func<InitiativeCreatedEventArgs, CancellationToken, Task> handler)
        {

        }


        private GetItemResult<ClaimsPrincipal> GetMessageOwner(Message message)
        {
            if (message == null)
                throw new ArgumentNullException("msg");

            var ownerClaimsResult = GetMessageString(message, propertyName: "OwnerClaims");
            var result = new GetItemResult<ClaimsPrincipal>();
            try
            {
                result.Item = CreatePrincipal(ownerClaimsResult.Item);
                _logger.Information("Message owner is {UserName} with email {Email}", result?.Item?.Identity?.Name, result?.Item?.GetEmail());
            }
            catch (Exception err)
            {
                string errorMessage = $"Unable to get Owner from token { ownerClaimsResult.Item }: { err.Message }";
                _logger.Error(err, "Unable to get Owner from token {Token}: {ErrorMessage}", ownerClaimsResult.Item, err.Message);
            }

            // last fail safe
            if (!result.WasMessageDeadLettered && result.Item == null)
            {
                string errorMessage = $"Unable to get Owner, reason unknown";
                _logger.Error(errorMessage);
            }

            return result;
        }

        private GetItemResult<string> GetMessageString(Message message, string propertyName, bool allowNullOrEmptyString = false)
        {
            if (message == null)
                throw new ArgumentNullException("msg");

            var result = GetMessageProperty<string>(message, propertyName: propertyName, allowNull: allowNullOrEmptyString);
            if (!result.WasMessageDeadLettered && !allowNullOrEmptyString)
            {
                if (string.IsNullOrWhiteSpace(result.Item))
                {
                    _logger.Error("{PropertyName} was empty in Service Bus message", propertyName);
                }
            }
            return result;
        }

        private GetItemResult<T> GetMessageProperty<T>(Message message, string propertyName, bool allowNull = false)
        {
            if (message == null)
                throw new ArgumentNullException("msg");

            var result = new GetItemResult<T>();
            if (!message.MessageProperties.ContainsKey(propertyName))
            {
                _logger.Error("{PropertyName} not found in message", propertyName);
            }
            else
            {
                object propertyObj = message.MessageProperties[propertyName];
                if (propertyObj == null)
                {
                    if (!allowNull)
                    {
                        _logger.Error("{PropertyName} was empty in Service Bus message", propertyName);
                    }
                    // else return null (or default) and don't dead letter
                }
                else
                {
                    try
                    {
                        result.Item = (T)propertyObj;
                    }
                    catch (Exception)
                    {
                        _logger.Error("{PropertyName} had value {Value}, which was not of the expected type '{Type}", propertyName, propertyObj, typeof(T).FullName);
                    }
                }
            }

            return result;
        }
    }
}

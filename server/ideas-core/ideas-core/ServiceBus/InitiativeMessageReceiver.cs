using CoE.Ideas.Core.Security;
using CoE.Ideas.Core.WordPress;
using Microsoft.Azure.ServiceBus;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CoE.Ideas.Core.ServiceBus
{
    //TODO: Mark messages as complete, abadoned or dead lettered!

    internal class InitiativeMessageReceiver : IInitiativeMessageReceiver
    {
        public InitiativeMessageReceiver(IIdeaRepository repository,
            IWordPressClient wordPressClient,
            ISubscriptionClient subscriptionClient,
            IJwtTokenizer jwtTokenizer)
        {
            _repository = repository ?? throw new ArgumentNullException("repository");
            _wordPressClient = wordPressClient ?? throw new ArgumentNullException("wordPressClient");
            _subscriptionClient = subscriptionClient ?? throw new ArgumentNullException("subscriptionClient");
            _jwtTokenizer = jwtTokenizer ?? throw new ArgumentNullException("jwtTokenizer");
        }
        private readonly IIdeaRepository _repository;
        private readonly IWordPressClient _wordPressClient;
        private readonly ISubscriptionClient _subscriptionClient;
        private readonly IJwtTokenizer _jwtTokenizer;


        public void ReceiveInitiativeCreated(Func<InitiativeCreatedEventArgs, CancellationToken, Task> handler, MessageHandlerOptions options)
        {
            _subscriptionClient.RegisterMessageHandler(async (msg, token) =>
            {
                // get the idea - InitiativeId specified by InitiativeCreatedSender
                var ideaTask = _repository.GetIdeaAsync((long)msg.UserProperties["InitiativeId"]);
                var owner = _jwtTokenizer.CreatePrincipal(msg.UserProperties["OwnerToken"] as string);
                var idea = await ideaTask;
                await handler(new InitiativeCreatedEventArgs()
                {
                    Initiative = ideaTask.Result,
                    Owner = owner
                }, token);
            }, options);
        }


        public void ReceiveInitiativeCreated(Func<InitiativeCreatedEventArgs, CancellationToken, Task> handler, 
            Func<ExceptionReceivedEventArgs, Task> exceptionReceivedHandler)
        {
            _subscriptionClient.RegisterMessageHandler(async (msg, token) =>
            {
                // get the idea - InitiativeId specified by InitiativeCreatedSender
                var ideaTask = _repository.GetIdeaAsync((long)msg.UserProperties["InitiativeId"]);
                var owner = _jwtTokenizer.CreatePrincipal(msg.UserProperties["OwnerToken"] as string);
                var idea = await ideaTask;
                await handler(new InitiativeCreatedEventArgs()
                {
                    Initiative = ideaTask.Result,
                    Owner = owner
                }, token);
            }, exceptionReceivedHandler);
        }

        public void ReceiveInitiativeWorkItemCreated(Func<WorkOrderCreatedEventArgs, CancellationToken, Task> handler, MessageHandlerOptions options)
        {
            _subscriptionClient.RegisterMessageHandler(async (msg, token) =>
            {
                // get the idea - InitiativeId specified by InitiativeCreatedSender
                var ideaTask = _repository.GetIdeaAsync((long)msg.UserProperties["InitiativeId"]);
                var owner = _jwtTokenizer.CreatePrincipal(msg.UserProperties["OwnerToken"] as string);
                var idea = await ideaTask;
                await handler(new WorkOrderCreatedEventArgs()
                {
                    Initiative = ideaTask.Result,
                    Owner = owner,
                    WorkOrderId = msg.UserProperties["WorkOrderId"] as string
                }, token);
            }, options);
        }

        public void ReceiveInitiativeWorkItemCreated(Func<WorkOrderCreatedEventArgs, CancellationToken, Task> handler, Func<ExceptionReceivedEventArgs, Task> exceptionReceivedHandler)
        {
            _subscriptionClient.RegisterMessageHandler(async (msg, token) =>
            {
                // get the idea - InitiativeId specified by InitiativeCreatedSender
                var ideaTask = _repository.GetIdeaAsync((long)msg.UserProperties["InitiativeId"]);
                var owner = _jwtTokenizer.CreatePrincipal(msg.UserProperties["OwnerToken"] as string);
                var idea = await ideaTask;
                await handler(new WorkOrderCreatedEventArgs()
                {
                    Initiative = ideaTask.Result,
                    Owner = owner,
                    WorkOrderId = msg.UserProperties["WorkOrderId"] as string
                }, token);
            }, exceptionReceivedHandler);
        }

        public void ReceiveWorkOrderUpdated(Func<WorkOrderUpdatedEventArgs, CancellationToken, Task> handler, MessageHandlerOptions options)
        {
            _subscriptionClient.RegisterMessageHandler(async (msg, token) =>
            {
                // not sure if the kind is preserved over the serialization of the service bus...
                var theDate = (DateTime)msg.UserProperties["WorkOrderUpdateTimeUtc"];
                await handler(new WorkOrderUpdatedEventArgs()
                {
                    UpdatedStatus = msg.UserProperties["WorkOrderStatus"] as string,
                    UpdatedDateUtc = theDate,
                    WorkOrderId = msg.UserProperties["WorkOrderId"] as string
                }, token);
            }, options);
        }

        public void ReceiveWorkOrderUpdated(Func<WorkOrderUpdatedEventArgs, CancellationToken, Task> handler, Func<ExceptionReceivedEventArgs, Task> exceptionReceivedHandler)
        {
            _subscriptionClient.RegisterMessageHandler(async (msg, token) =>
            {
                // not sure if the kind is preserved over the serialization of the service bus...
                var theDate = (DateTime)msg.UserProperties["WorkOrderUpdateTimeUtc"];
                await handler(new WorkOrderUpdatedEventArgs()
                {
                    UpdatedStatus = msg.UserProperties["WorkOrderStatus"] as string,
                    UpdatedDateUtc = theDate,
                    WorkOrderId = msg.UserProperties["WorkOrderId"] as string
                }, token);
            }, exceptionReceivedHandler);
        }

        public void ReceiveInitiativeLogged(Func<InitiativeLoggedEventArgs, CancellationToken, Task> handler, MessageHandlerOptions options)
        {
            _subscriptionClient.RegisterMessageHandler(async (msg, token) =>
            {
                // get the idea - InitiativeId specified by InitiativeCreatedSender
                var ideaTask = _repository.GetIdeaAsync((long)msg.UserProperties["InitiativeId"]);
                var owner = _jwtTokenizer.CreatePrincipal(msg.UserProperties["OwnerToken"] as string);
                var idea = await ideaTask;
                await handler(new InitiativeLoggedEventArgs()
                {
                    Initiative = ideaTask.Result,
                    Owner = owner,
                    RangeUpdated = msg.UserProperties["RangeUpdated"] as string
                }, token);
            }, options);
        }

        public void ReceiveInitiativeLogged(Func<InitiativeLoggedEventArgs, CancellationToken, Task> handler, Func<ExceptionReceivedEventArgs, Task> exceptionReceivedHandler)
        {
            _subscriptionClient.RegisterMessageHandler(async (msg, token) =>
            {
                try
                {
                    // get the idea - InitiativeId specified by InitiativeCreatedSender
                    var ideaTask = _repository.GetIdeaAsync((long)msg.UserProperties["InitiativeId"]);
                    var owner = _jwtTokenizer.CreatePrincipal(msg.UserProperties["OwnerToken"] as string);
                    var idea = await ideaTask;
                    await handler(new InitiativeLoggedEventArgs()
                    {
                        Initiative = ideaTask.Result,
                        Owner = owner,
                        RangeUpdated = msg.UserProperties["RangeUpdated"] as string
                    }, token);
                }
                catch (Exception err)
                {
                    throw;
                }
            }, exceptionReceivedHandler);
        }
    }
}

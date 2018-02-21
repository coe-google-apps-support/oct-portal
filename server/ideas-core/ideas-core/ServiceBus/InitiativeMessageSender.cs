using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CoE.Ideas.Core.Security;
using CoE.Ideas.Core.WordPress;
using Microsoft.Azure.ServiceBus;

namespace CoE.Ideas.Core.ServiceBus
{
    public class InitiativeMessageSender : IInitiativeMessageSender
    {
        public InitiativeMessageSender(ITopicClient topicClient, 
            IJwtTokenizer jwtTokenizer)
        {
            _topicClient = topicClient ?? throw new ArgumentNullException("topicClient");
            _jwtTokenizer = jwtTokenizer ?? throw new ArgumentNullException("jwtTokenizer");
        }

        private readonly ITopicClient _topicClient;
        private readonly IJwtTokenizer _jwtTokenizer;


        public Task SendInitiativeCreatedAsync(InitiativeCreatedEventArgs args)
        {
            if (args == null)
                throw new ArgumentNullException("args");
            if (args.Initiative == null)
                throw new ArgumentException("Initiative cannot be null");
            if (args.Owner == null)
                throw new ArgumentException("Owner cannot be null");

            var message = new Message
            {
                Label = "Initiative Created"
            };
            message.UserProperties["InitiativeId"] = args.Initiative.Id;
            //message.UserProperties["OwnerClaims"] = ownerPrincipal.Claims;
            message.UserProperties["OwnerToken"] = _jwtTokenizer.CreateJwt(args.Owner);

            return _topicClient.SendAsync(message);
        }


        public Task SendInitiativeWorkOrderCreatedAsync(WorkOrderCreatedEventArgs args)
        {
            if (args == null)
                throw new ArgumentNullException("args");
            if (args.Initiative == null)
                throw new ArgumentException("Initiative cannot be null");
            if (args.Owner == null)
                throw new ArgumentException("Owner cannot be null");
            if (string.IsNullOrWhiteSpace(args.WorkOrderId))
                throw new ArgumentException("WorkOrderId cannot be null or empty");

            var returnMessage = new Message
            {
                Label = "Remedy Work Item Created"
            };
            returnMessage.UserProperties["InitiativeId"] = args.Initiative.Id;
            returnMessage.UserProperties["OwnerToken"] = _jwtTokenizer.CreateJwt(args.Owner);
            returnMessage.UserProperties["WorkOrderId"] = args.WorkOrderId;
            return _topicClient.SendAsync(returnMessage);
        }

        public Task SendWorkOrderUpdatedAsync(WorkOrderUpdatedEventArgs args)
        {
            if (args == null)
                throw new ArgumentNullException("args");
            if (string.IsNullOrWhiteSpace(args.WorkOrderId))
                throw new ArgumentException("WorkOrderId cannot be null");
            if (string.IsNullOrWhiteSpace(args.UpdatedStatus))
                throw new ArgumentException("UpdatedStatus cannot be null");

            if (args.UpdatedDateUtc > DateTime.UtcNow)
                throw new ArgumentOutOfRangeException($"UpdatedDateUtc cannot be in the future ({ args.UpdatedDateUtc.ToLocalTime() })");


            var message = new Message
            {
                Label = "Work Order Updated"
            };
            message.UserProperties["WorkOrderId"] = args.WorkOrderId;
            message.UserProperties["WorkOrderStatus"] = args.UpdatedStatus;
            message.UserProperties["WorkOrderUpdateTimeUtc"] = args.UpdatedDateUtc;

            return _topicClient.SendAsync(message);
        }

        public Task SendInitiativeLoggedAsync(InitiativeLoggedEventArgs args)
        {
            if (args == null)
                throw new ArgumentNullException("args");
            if (args.Initiative == null)
                throw new ArgumentException("Initiative cannot be null");
            if (args.Owner == null)
                throw new ArgumentException("Owner cannot be null");

            // note args.RangeUpdated is allowed to be null

            var message = new Message()
            {
                Label = "Initiative Logged"
            };
            message.UserProperties["InitiativeId"] = args.Initiative.Id;
            message.UserProperties["OwnerToken"] = _jwtTokenizer.CreateJwt(args.Owner);
            message.UserProperties["RangeUpdated"] = args.RangeUpdated;
            return _topicClient.SendAsync(message);
        }
    }
}

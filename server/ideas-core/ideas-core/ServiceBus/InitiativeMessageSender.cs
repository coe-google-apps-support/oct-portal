using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CoE.Ideas.Core.Security;
using CoE.Ideas.Core.WordPress;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;

namespace CoE.Ideas.Core.ServiceBus
{
    public class InitiativeMessageSender : IInitiativeMessageSender
    {
        public InitiativeMessageSender(ITopicClient topicClient)
        {
            _topicClient = topicClient ?? throw new ArgumentNullException("topicClient");
        }

        private readonly ITopicClient _topicClient;


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
            message.UserProperties["OwnerClaims"] = SerializeUser(args.Owner);

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
            returnMessage.UserProperties["OwnerClaims"] = SerializeUser(args.Owner);
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
            message.UserProperties["WorkOrderAssigneeEmail"] = args.AssigneeEmail;
            message.UserProperties["WorkOrderAssigneeDisplayName"] = args.AssigneeDisplayName;

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
            message.UserProperties["OwnerClaims"] = SerializeUser(args.Owner);
            message.UserProperties["RangeUpdated"] = args.RangeUpdated;
            return _topicClient.SendAsync(message);
        }


        private string SerializeUser(ClaimsPrincipal claimsPrincipal)
        {
            if (claimsPrincipal == null)
                return string.Empty;

            var claims = claimsPrincipal.Claims.Select(x => new KeyValuePair<string, string>(x.Type, x.Value)).ToArray();
            return JsonConvert.SerializeObject(claims);
        }
    }
}

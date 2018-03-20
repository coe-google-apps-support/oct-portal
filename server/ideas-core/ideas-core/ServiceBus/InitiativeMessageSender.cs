using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CoE.Ideas.Core.Data;
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

        internal const string INITIATIVE_CREATED = "Initiative Created";
        internal const string REMEDY_WORK_ITEM_CREATED = "Remedy Work Item Created";
        internal const string WORK_ORDER_UPDATED = "Work Order Updated";
        internal const string INITIATIVE_LOGGED = "Initiative Logged";

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
                Label = INITIATIVE_CREATED
            };
            SetInitiative(args.Initiative, message.UserProperties);
            SetOwner(args.Owner, message.UserProperties);
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
                Label = REMEDY_WORK_ITEM_CREATED
            };
            SetInitiative(args.Initiative, returnMessage.UserProperties);
            SetOwner(args.Owner, returnMessage.UserProperties);
            SetWorkOrder(args.WorkOrderId, returnMessage.UserProperties);
            return _topicClient.SendAsync(returnMessage);
        }

        internal static void SetInitiative(Initiative initiative, IDictionary<string, object> dictionary)
        {
            dictionary["InitiativeId"] = initiative.Id;
        }

        internal static void SetOwner(ClaimsPrincipal owner, IDictionary<string, object> dictionary)
        {
            dictionary["OwnerClaims"] = SerializeUser(owner);
        }

        internal static void SetWorkOrder(string workOrderId, IDictionary<string, object> dictionary)
        {
            dictionary["WorkOrderId"] = workOrderId;
        }


        internal static void SetWorkOrder(string workOrderId, 
            string updatedStatus,
            DateTime updateDateUtc,
            string assigneeEmail,
            string assigneeDisplayName,
            IDictionary<string, object> dictionary)
        {
            dictionary["WorkOrderId"] = workOrderId;
            dictionary["WorkOrderStatus"] = updatedStatus;
            dictionary["WorkOrderUpdateTimeUtc"] = updateDateUtc;
            dictionary["WorkOrderAssigneeEmail"] = assigneeEmail;
            dictionary["WorkOrderAssigneeDisplayName"] = assigneeDisplayName;
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
                Label = WORK_ORDER_UPDATED
            };
            SetWorkOrder(args.WorkOrderId, 
                args.UpdatedStatus, 
                args.UpdatedDateUtc, 
                args.AssigneeEmail, 
                args.AssigneeDisplayName,  
                message.UserProperties);

            return _topicClient.SendAsync(message);
        }

        internal static void SetRangeUpdated(string rangeUpdated, IDictionary<string, object> dictionary)
        {
            dictionary["RangeUpdated"] = rangeUpdated;
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
                Label = INITIATIVE_LOGGED
            };
            SetInitiative(args.Initiative, message.UserProperties);
            SetOwner(args.Owner, message.UserProperties);
            SetRangeUpdated(args.RangeUpdated, message.UserProperties);
            return _topicClient.SendAsync(message);
        }


        private static string SerializeUser(ClaimsPrincipal claimsPrincipal)
        {
            if (claimsPrincipal == null)
                return string.Empty;

            var claims = claimsPrincipal.Claims.Select(x => new KeyValuePair<string, string>(x.Type, x.Value)).ToArray();
            return JsonConvert.SerializeObject(claims);
        }
    }
}

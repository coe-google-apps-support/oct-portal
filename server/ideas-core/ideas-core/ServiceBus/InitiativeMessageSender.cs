using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CoE.Ideas.Core.Data;
using EnsureThat;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;

namespace CoE.Ideas.Core.ServiceBus
{
    internal class InitiativeMessageSender : IInitiativeMessageSender
    {
        public InitiativeMessageSender(IMessageSender messageSender)
        {
            EnsureArg.IsNotNull(messageSender);
            _messageSender = messageSender;
        }

        private readonly IMessageSender _messageSender;

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

            var userProperties = new Dictionary<string, object>();
            SetInitiative(args.Initiative, userProperties);
            SetOwner(args.Owner, userProperties);
            return _messageSender.SendMessageAsync(INITIATIVE_CREATED, userProperties);
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

            var userProperties = new Dictionary<string, object>();
            SetInitiative(args.Initiative, userProperties);
            SetOwner(args.Owner, userProperties);
            SetWorkOrder(args.WorkOrderId, userProperties);
            return _messageSender.SendMessageAsync(REMEDY_WORK_ITEM_CREATED, userProperties);
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


            var userProperties = new Dictionary<string, object>();

            SetWorkOrder(args.WorkOrderId, 
                args.UpdatedStatus, 
                args.UpdatedDateUtc, 
                args.AssigneeEmail, 
                args.AssigneeDisplayName,
                userProperties);

            return _messageSender.SendMessageAsync(WORK_ORDER_UPDATED, userProperties);
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

            var userProperties = new Dictionary<string, object>();
            SetInitiative(args.Initiative, userProperties);
            SetOwner(args.Owner, userProperties);
            SetRangeUpdated(args.RangeUpdated, userProperties);
            return _messageSender.SendMessageAsync(INITIATIVE_LOGGED, userProperties);
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

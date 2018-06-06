﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CoE.Ideas.Core.Data;
using EnsureThat;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using Serilog;

namespace CoE.Ideas.Core.ServiceBus
{
    internal class InitiativeMessageSender : IInitiativeMessageSender
    {
        public InitiativeMessageSender(IMessageSender messageSender,
            ILogger logger)
        {
            EnsureArg.IsNotNull(messageSender);
            EnsureArg.IsNotNull(logger);
            _messageSender = messageSender;
            _logger = logger;
        }

        private readonly IMessageSender _messageSender;
        private readonly ILogger _logger;

        internal const string INITIATIVE_CREATED = "Initiative Created";
        internal const string REMEDY_WORK_ITEM_CREATED = "Remedy Work Item Created";
        internal const string WORK_ORDER_UPDATED = "Work Order Updated";
        internal const string INITIATIVE_LOGGED = "Initiative Logged";
        internal const string STATUS_CHANGED = "Status Changed";
        internal const string STATUS_DESCRIPTION_CHANGED = "Status Description Changed";

        public Task SendInitiativeCreatedAsync(InitiativeCreatedEventArgs args)
        {
            if (args == null)
                throw new ArgumentNullException("args");
            if (args.Initiative == null)
                throw new ArgumentException("Initiative cannot be null");
            if (args.Owner == null)
                throw new ArgumentException("Owner cannot be null");

            _logger.Information("Posting InitiativeCreated event to service bus for Initiative {InitiativeId}", args.Initiative.Id);

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

            _logger.Information("Posting InitiativeWorkOrderCreated event to service bus for Initiative {InitiativeId} and WorkOrderId {WorkOrderId}", args.Initiative.Id, args.WorkOrderId);

            var userProperties = new Dictionary<string, object>();
            SetInitiative(args.Initiative, userProperties);
            SetOwner(args.Owner, userProperties);
            SetWorkOrder(args.WorkOrderId, args.EtaUtc, userProperties);
            //ADD ETA!
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

        internal static void SetWorkOrder(string workOrderId, DateTime? etaUtc, IDictionary<string, object> dictionary)
        {
            dictionary[nameof(WorkOrderCreatedEventArgs.WorkOrderId)] = workOrderId;
            dictionary[nameof(WorkOrderCreatedEventArgs.EtaUtc)] = etaUtc;
        }


        internal static void SetWorkOrder(string remedyStatus,
            string updatedStatus,
            DateTime updateDateUtc,
            string assigneeEmail,
            string assigneeDisplayName,
            IDictionary<string, object> dictionary)
        {
            dictionary[nameof(WorkOrderUpdatedEventArgs.RemedyStatus)] = remedyStatus;
            dictionary[nameof(WorkOrderUpdatedEventArgs.UpdatedStatus)] = updatedStatus;
            dictionary[nameof(WorkOrderUpdatedEventArgs.UpdatedDateUtc)] = updateDateUtc;
            dictionary[nameof(WorkOrderUpdatedEventArgs.AssigneeEmail)] = assigneeEmail;
            dictionary[nameof(WorkOrderUpdatedEventArgs.AssigneeDisplayName)] = assigneeDisplayName;
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

            _logger.Information("Posting WorkOrderUpdated event to service bus for Work Order {WorkOrderId}, updated by {AssigneeEmail}", args.WorkOrderId, args.AssigneeEmail);

            var userProperties = new Dictionary<string, object>();

            SetWorkOrder(args.WorkOrderId, args.EtaUtc, userProperties);
            SetWorkOrder(args.RemedyStatus,
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

            _logger.Information("Posting InitiativeLogged event to service bus for Initiative {InitiativeId}", args.Initiative.Id);

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

        public Task SendInitiativeStatusChangedAsync(InitiativeStatusChangedEventArgs args)
        {
            if (args == null)
                throw new ArgumentNullException("args");
            if (args.Initiative == null)
                throw new ArgumentException("Initiative cannot be null");

            var userProperties = new Dictionary<string, object>();
            SetInitiative(args.Initiative, userProperties);
            return _messageSender.SendMessageAsync(STATUS_CHANGED, userProperties);
        }

        public Task SendInitiativeStatusDescriptionChangedAsync(InitiativeStatusDescriptionChangedEventArgs args)
        {
            if (args == null)
                throw new ArgumentNullException("args");
            if (args.Initiative == null)
                throw new ArgumentException("Initiative cannot be null");

            var userProperties = new Dictionary<string, object>();
            SetInitiative(args.Initiative, userProperties);
            SetOwner(args.Owner, userProperties);
            return _messageSender.SendMessageAsync(STATUS_DESCRIPTION_CHANGED, userProperties);
        }
    }
}

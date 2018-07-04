using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CoE.Issues.Core.Data;
using EnsureThat;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using Serilog;

namespace CoE.Issues.Core.ServiceBus
{
    internal class IssueMessageSender: IIssueMessageSender
    {

        public IssueMessageSender(IMessageSender messageSender,
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

        public Task SendIssueCreatedAsync(IssueCreatedEventArgs args)
        {
            if (args == null)
                throw new ArgumentNullException("args");
            if (args.Issue == null)
                throw new ArgumentException("Initiative cannot be null");
            if (args.Owner == null)
                throw new ArgumentException("Owner cannot be null");

            _logger.Information("Posting IssueCreated event to service bus for Issue {IssueId}", args.Issue.Id);

            var userProperties = new Dictionary<string, object>();
            SetIssue(args.Issue, userProperties);
            SetOwner(args.Owner, userProperties);
            return _messageSender.SendMessageAsync(INITIATIVE_CREATED, userProperties);
        }


        public Task SendIssueIncidentCreatedAsync(IncidentCreatedEventArgs args)
        {
            if (args == null)
                throw new ArgumentNullException("args");
            if (args.Issue == null)
                throw new ArgumentException("Initiative cannot be null");
            if (args.Owner == null)
                throw new ArgumentException("Owner cannot be null");
            if (string.IsNullOrWhiteSpace(args.IncidentId))
                throw new ArgumentException("IncidentId cannot be null or empty");

            _logger.Information("Posting InitiativeWorkOrderCreated event to service bus for Initiative {InitiativeId} and IncidentId {IncidentId}", args.Issue.Id, args.IncidentId);

            var userProperties = new Dictionary<string, object>();
            SetIssue(args.Issue, userProperties);
            SetOwner(args.Owner, userProperties);
            SetWorkOrder(args.IncidentId, args.EtaUtc, userProperties);
            //ADD ETA!
            return _messageSender.SendMessageAsync(REMEDY_WORK_ITEM_CREATED, userProperties);
        }

        internal static void SetIssue(Issue issue, IDictionary<string, object> dictionary)
        {
            dictionary["IssueId"] = issue.Id;
        }

        internal static void SetOwner(ClaimsPrincipal owner, IDictionary<string, object> dictionary)
        {
            dictionary["OwnerClaims"] = SerializeUser(owner);
        }

        internal static void SetWorkOrder(string IncidentId, DateTime? etaUtc, IDictionary<string, object> dictionary)
        {
            dictionary[nameof(IncidentCreatedEventArgs.IncidentId)] = IncidentId;
            dictionary[nameof(IncidentCreatedEventArgs.EtaUtc)] = etaUtc;
        }


        internal static void SetWorkOrder(string remedyStatus,
            string updatedStatus,
            DateTime updateDateUtc,
            string assigneeEmail,
            string assigneeDisplayName,
            IDictionary<string, object> dictionary)
        {
            dictionary[nameof(IncidentUpdatedEventArgs.RemedyStatus)] = remedyStatus;
            dictionary[nameof(IncidentUpdatedEventArgs.UpdatedStatus)] = updatedStatus;
            dictionary[nameof(IncidentUpdatedEventArgs.UpdatedDateUtc)] = updateDateUtc;
            dictionary[nameof(IncidentUpdatedEventArgs.AssigneeEmail)] = assigneeEmail;
            dictionary[nameof(IncidentUpdatedEventArgs.AssigneeDisplayName)] = assigneeDisplayName;
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

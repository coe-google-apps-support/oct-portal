using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CoE.Ideas.Shared.ServiceBus;
using CoE.Issues.Core.Data;
using EnsureThat;
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

        internal const string ISSUE_CREATED = "Issue Created";
        internal const string New_ISSUE_CREATED = "New Issue Created";

        public Task SendIssueCreatedAsync(IssueCreatedEventArgs args)
        {
            if (args == null)
                throw new ArgumentNullException("args");

            _logger.Information("Posting IssueCreated event to service bus.");

            var userProperties = new Dictionary<string, object>();
            userProperties["Title"] = args.Title;
            userProperties["Description"] = args.Description;
            userProperties["RemedyStatus"] = args.RemedyStatus;
            userProperties["RequestorGivenName"] = args.RequestorGivenName;
            userProperties["RequestorSurnName"] = args.RequestorSurnName;
            userProperties["RequestorDisplayName"] = args.RequestorDisplayName;
            userProperties["RequestorTelephone"] = args.RequestorTelephone;
            userProperties["RequestorEmail"] = args.RequestorEmail;
            userProperties["ReferenceId"] = args.ReferenceId;
            userProperties["AssigneeEmail"] = args.AssigneeEmail;
            userProperties["AssigneeGroup"] = args.AssigneeGroup;
            userProperties["CreatedDate"] = args.CreatedDate;
            userProperties["Urgency"] = args.Urgency;


            return _messageSender.SendMessageAsync(ISSUE_CREATED, userProperties);
        }

        public Task SendNewIssueCreatedAsync(NewIssueCreatedEventArgs args)
        {
            if (args == null)
                throw new ArgumentNullException("args");
            if (args.Issue == null)
                throw new ArgumentException("Issue cannot be null");
            if (args.Owner == null)
                throw new ArgumentException("Owner cannot be null");

            _logger.Information("Posting IssueCreated event to service bus for Issue {IssueId}", args.Issue.Id);

            var userProperties = new Dictionary<string, object>();
            SetIssue(args.Issue, userProperties);
            SetOwner(args.Owner, userProperties);
            return _messageSender.SendMessageAsync(New_ISSUE_CREATED, userProperties);
        }

        internal static void SetIssue(Issue issue, IDictionary<string, object> dictionary)
        {
            dictionary["Issue"] = issue.Id;
        }
        internal static void SetOwner(ClaimsPrincipal owner, IDictionary<string, object> dictionary)
        {
            dictionary["OwnerClaims"] = SerializeUser(owner);
        }

        internal static void SetWorkOrder(string incidentId,  IDictionary<string, object> dictionary)
        {
            dictionary[nameof(NewIssueCreatedEventArgs.IncidentId)] = incidentId;
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

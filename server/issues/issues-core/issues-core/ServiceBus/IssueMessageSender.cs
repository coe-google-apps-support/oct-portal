using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
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
        internal const string INCIDENT_CREATED = "Incident Created";

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
            userProperties["ReferenceId"] = args.ReferenceId;
            userProperties["AssigneeEmail"] = args.AssigneeEmail;
            userProperties["RequestorEmail"] = args.RequestorEmail;

            return _messageSender.SendMessageAsync(ISSUE_CREATED, userProperties);
        }


        internal static void SetIssue(Issue issue, IDictionary<string, object> dictionary)
        {
            dictionary["Issue"] = JsonConvert.SerializeObject(issue);
        }
    }
}

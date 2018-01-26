using CoE.Ideas.Core.ProjectManagement;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoE.Ideas.ProjectManagement.Core.GitHub
{
    public class GitHubIssueEvent : ProjectManagementEntityBase
    {
        [JsonProperty("event_id")]
        public override long Id { get => base.Id; set => base.Id = value; }
        [JsonProperty("id")]
        public string AlternateKey { get; set; }
        [JsonProperty("action")]
        public string Action { get; set; }
        [JsonProperty("event")]
        public string EventName { get; set; }
        [JsonProperty("issue")]
        public GitHubIssue Issue { get; set; }
        [JsonProperty("repository")]
        public GitHubRepository Repository { get; set; }
        [JsonProperty("sender")]
        public GitHubUser Sender { get; set; }
        [JsonProperty("assignee")]
        public GitHubUser Assignee { get; set; }
        [JsonProperty("assigner")]
        public GitHubUser Assigner { get; set; }
        [JsonProperty("url")]
        public string Url { get; set; }
    }
}

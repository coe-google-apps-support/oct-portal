using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoE.Ideas.ProjectManagement.Core.GitHub
{
    public class GitHubIssueEvent : Issue
    {
        [JsonProperty("action")]
        public string Action { get; set; }
        [JsonProperty("issue")]
        public GitHubIssue Issue { get; set; }
        [JsonProperty("repository")]
        public GitHubRepository Repository { get; set; }
        [JsonProperty("sender")]
        public GitHubUser Sender { get; set; }
    }
}

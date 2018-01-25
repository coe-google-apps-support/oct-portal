using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoE.Ideas.ProjectManagement.Core.GitHub
{
    public class GitHubIssue : Issue
    {

        [JsonProperty("issue_id")]
        public override long Id { get => base.Id; set => base.Id = value; }

        [JsonProperty("labels_url")]
        public string LabelsUrl { get; set; }

        [JsonProperty("comments_url")]
        public string CommentsUrl { get; set; }

        [JsonProperty("events_url")]
        public string EventsUrl { get; set; }

        [JsonProperty("html_url")]
        public string HtmlUrl { get; set; }

        [JsonProperty("id")]
        public override string AlternateKey { get => base.AlternateKey; set => base.AlternateKey = value; }

        [JsonProperty("number")]
        public int Number { get; set; }

        [JsonProperty("title")]
        public override string Title { get => base.Title; set => base.Title = value; }

        [JsonProperty("user")]
        public GitHubUser User { get; set; }

        [JsonProperty("labels")]
        public ICollection<GitHubLabel> Labels { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }

        [JsonProperty("locked")]
        public bool IsLocked { get; set; }

        [JsonProperty("assignee")]
        public GitHubUser Assignee { get; set; }

        [JsonProperty("assignees")]
        public ICollection<GitHubUser> Asseignees { get; set; }

        [JsonProperty("milestone")]
        public string Milestone { get; set; }

        [JsonProperty("comments")]
        public string Comments { get; set; }

        [JsonProperty("created_at")]
        public override DateTimeOffset CreatedDate { get => base.CreatedDate; set => base.CreatedDate = value; }

        [JsonProperty("updated_at")]
        public DateTimeOffset UpdatedAt { get; set; }

        [JsonProperty("closed_at")]
        public DateTimeOffset? ClosedAt { get; set; }

        [JsonProperty("body")]
        public string Body { get; set; }

        [JsonProperty("repository")]
        public GitHubRepository Repository { get; set; }

        [JsonProperty("sender")]
        public GitHubUser Sender { get; set; }
    }
}

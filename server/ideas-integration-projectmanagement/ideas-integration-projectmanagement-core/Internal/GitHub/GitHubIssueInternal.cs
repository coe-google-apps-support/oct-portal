using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CoE.Ideas.ProjectManagement.Core.Internal.GitHub
{
    // ref: https://developer.github.com/v3/activity/events/types/#issuesevent
    [Table("Issues", Schema = "GitHub")]
    internal class GitHubIssueInternal : IssueInternal
    {
        public string LabelsUrl { get; set; }
        public string CommentsUrl { get; set; }
        public string EventsUrl { get; set; }
        public string HtmlUrl { get; set; }
        public int Number { get; set; }
        public GitHubUserInternal User { get; set; }
        public ICollection<GitHubLabelInternal> Labels { get; set; }
        public string State { get; set; }
        public bool IsLocked { get; set; }
        public string  Assignee { get; set; }
        public string Milestone { get; set; }
        public string Comments { get; set; }
        // CreatedAt is in base class (CreatedDate)
        //public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
        public DateTimeOffset? ClosedAt { get; set; }
        public string Body { get; set; }
    }
}

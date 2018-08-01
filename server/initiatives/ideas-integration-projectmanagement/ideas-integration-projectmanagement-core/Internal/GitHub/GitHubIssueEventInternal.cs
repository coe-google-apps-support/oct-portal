using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CoE.Ideas.ProjectManagement.Core.Internal.GitHub
{
    [Table("GitHub_Issue_Events")]
    internal class GitHubIssueEventInternal : ProjectManagementEntityBaseInternal
    {
        public string Action { get; set; }
        public GitHubIssueInternal Issue { get; set; }
        public GitHubRepositoryInternal Repository { get; set; }
        public GitHubUserInternal Sender { get; set; }
        public GitHubUserInternal Assignee { get; set; }
        public GitHubUserInternal Assigner { get; set; }
        public string Url { get; set; }

    }
}

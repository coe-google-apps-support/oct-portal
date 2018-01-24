using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CoE.Ideas.ProjectManagement.Core.Internal.GitHub
{
    [Table("IssueEvents", Schema = "GitHub")]
    internal class GitHubIssueEventInternal : ProjectManagementEntityBaseInternal
    {
        public string Action { get; set; }
        public GitHubIssueInternal Issue { get; set; }
        public GitHubRepositoryInternal Repository { get; set; }
        public GitHubUserInternal Sender { get; set; }

    }
}

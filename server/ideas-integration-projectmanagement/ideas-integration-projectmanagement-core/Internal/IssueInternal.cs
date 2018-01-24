using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CoE.Ideas.ProjectManagement.Core.Internal
{
    [Table("Issues")]
    internal abstract class IssueInternal : ProjectManagementEntityBaseInternal
    {
        public ProjectManagementSystemInternal ProjectManagementSystem { get; set; }
        public string AlternateKey { get; set; }
        public string Url { get; set; }
        public string Title { get; set; }
        public IssueStatusInternal IssueStatus { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
    }
}

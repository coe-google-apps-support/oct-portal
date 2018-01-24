using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CoE.Ideas.ProjectManagement.Core.Internal.GitHub
{
    [Table("Labels", Schema = "GitHub")]
    internal class GitHubLabelInternal : ProjectManagementEntityBaseInternal
    {
        public string AlternateKey { get; set; }
        public string Url { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
        public bool IsDefault { get; set; }
    }
}

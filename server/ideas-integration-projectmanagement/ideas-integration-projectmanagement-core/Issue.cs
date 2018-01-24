using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoE.Ideas.ProjectManagement.Core
{
    public class Issue : ProjectManagementEntityBase
    {
        public virtual string Url { get; set; }

        public ProjectManagementSystem ProjectManagementSystem { get; set; }
        public virtual string AlternateKey { get; set; }
        public virtual string Title { get; set; }
        public IssueStatus IssueStatus { get; set; }
        public virtual DateTimeOffset CreatedDate { get; set; }

    }
}

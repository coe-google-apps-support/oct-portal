using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CoE.Ideas.ProjectManagement.Core.Internal
{
    [Table("Issue_Status_Changes")]
    internal class IssueStatusChangeInternal : ProjectManagementEntityBaseInternal
    {
        public IssueInternal Issue { get; set; }
        public IssueStatusInternal NewStatus { get; set; }
        public DateTimeOffset ChangeDate { get; set; }
    }
}

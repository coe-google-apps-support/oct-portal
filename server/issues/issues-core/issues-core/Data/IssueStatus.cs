using System;
using System.Collections.Generic;
using System.Text;

namespace CoE.Issues.Core.Data
{
    public enum IssueStatus
    {
        Cancelled = 1,
        Initiate = 2,
        Submit = 3,
        Review = 4,
        Collaborate = 5,
        Deliver = 6,
        Closed = 7,
    }
}

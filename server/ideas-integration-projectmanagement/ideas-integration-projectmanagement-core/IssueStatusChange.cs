﻿using System;
using System.Collections.Generic;
using System.Text;

namespace CoE.Ideas.ProjectManagement.Core
{
    public class IssueStatusChange : ProjectManagementEntityBase
    {
        public Issue Issue { get; set; }
        public IssueStatus NewStatus { get; set; }
        public DateTimeOffset ChangeDate { get; set; }
    }
}
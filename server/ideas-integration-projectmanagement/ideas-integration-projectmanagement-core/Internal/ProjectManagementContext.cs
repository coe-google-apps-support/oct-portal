using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoE.Ideas.ProjectManagement.Core.Internal
{
    internal class ProjectManagementContext : DbContext
    {
        public ProjectManagementContext(DbContextOptions options) : base(options)
        { }

        public DbSet<IssueInternal> Issues { get; set; }
        public DbSet<IssueStatusChangeInternal> IssueStatusChanges { get; set; }
    }
}

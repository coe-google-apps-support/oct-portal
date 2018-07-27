using System;
using System.Collections.Generic;
using System.Text;
using CoE.Ideas.ProjectManagement.Core.Internal.GitHub;
using Microsoft.EntityFrameworkCore;

namespace CoE.Ideas.ProjectManagement.Core.Internal
{
    internal class ExtendedProjectManagementContext : ProjectManagementContext
    {
        public ExtendedProjectManagementContext(DbContextOptions<ExtendedProjectManagementContext> options) : base(options)
        { }

        public DbSet<GitHubIssueEventInternal> GitHubIssueEvents { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // we have to tell the model this type is part of the model
            modelBuilder.Entity<GitHubIssueInternal>();
        }
    }
}

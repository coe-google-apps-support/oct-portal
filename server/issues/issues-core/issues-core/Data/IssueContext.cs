using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoE.Issues.Core.Data
{
    internal class IssueContext : DbContext
#if DEBUG
     , Microsoft.EntityFrameworkCore.Design.IDesignTimeDbContextFactory<IssueContext>
#endif
    {

        #region Constructors and constants
        public IssueContext(
            DbContextOptions<IssueContext> options,
            Serilog.ILogger logger) : base(options)
        {
            _logger = logger;
        }

        private readonly Serilog.ILogger _logger;
        #endregion


        public DbSet<Issue> Issues { get; set; }


#if DEBUG
        // This section adds Suport for Entity Framework PowerShell commands like Add-Migration
        // examples: 
        //     - Update-Database -Context IssueContext -Project issues-core -StartupProject issues-core 
        //     - Add-Migration -Context IssueContext -Project issues-core -StartupProject issues-core -Name MyNewMigration
        public IssueContext() { }


        public IssueContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<IssueContext>();
            builder.UseMySql("server=127.0.0.1;uid=root;pwd=octavadev;database=issues");

            return new IssueContext(builder.Options, null);
        }
#endif

    }
}

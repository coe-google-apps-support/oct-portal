using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoE.Ideas.Core.Internal.Initiatives
{
    internal class IdeaContext : DbContext
    {
        public IdeaContext(DbContextOptions<IdeaContext> options) : base(options)
        { }

        public DbSet<IdeaInternal> Ideas { get; set; }

        public DbSet<TagInternal> Tags { get; set; }

        public DbSet<BranchInternal> Branches { get; set; }

        public DbSet<DepartmentInternal> Departments { get; set; }

        public DbSet<PersonInternal> People { get; set; }

        public DbSet<StringTemplateInternal> StringTempltaes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // set default values
            modelBuilder.Entity<BranchInternal>()
                .Property(b => b.IsActive)
                .HasDefaultValue(true);

            modelBuilder.Entity<DepartmentInternal>()
                .Property(d => d.IsActive)
                .HasDefaultValue(true);
        }
    }
}

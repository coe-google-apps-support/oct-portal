using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoE.Ideas.Core.Internal
{
    internal class IdeaContext : DbContext
    {
        public IdeaContext(DbContextOptions<IdeaContext> options) : base(options)
        { }

        public DbSet<IdeaInternal> Ideas { get; set; }

        public DbSet<TagInternal> Tags { get; set; }

    }
}

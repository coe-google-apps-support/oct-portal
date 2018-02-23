using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoE.Ideas.Core.Internal.WordPress
{
    internal class WordPressContext : DbContext
    {
        public WordPressContext(DbContextOptions<WordPressContext> options) : base(options)
        { }


        public DbSet<UserInternal> Users { get; set; }
        public DbSet<UserMetadataInternal> UserMetadata { get; set; }
    }
}

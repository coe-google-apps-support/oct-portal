using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoE.Ideas.Shared.WordPress
{
    internal class WordPressContext : DbContext
    {
        public WordPressContext(DbContextOptions<WordPressContext> options) : base(options)
        { }


        public DbSet<User> Users { get; set; }
        public DbSet<UserMetadata> UserMetadata { get; set; }

    }

}

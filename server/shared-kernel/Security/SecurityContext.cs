using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoE.Ideas.Shared.Security
{
    internal class SecurityContext : DbContext
    {
        public SecurityContext(DbContextOptions<SecurityContext> options) : base(options)
        { }


        public DbSet<PermissionRole> Permissions { get; set; }
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoE.Ideas.Shared.ServiceBus
{
    internal class ServiceBusEmulatorContext : DbContext
    {
        public ServiceBusEmulatorContext() : base() { }

        public ServiceBusEmulatorContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<Message> Messages { get; set; }

        public DbSet<MessageProperty> MessageProperties { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Message>()
                .Ignore(x => x.MessageProperties);

            base.OnModelCreating(modelBuilder);
        }
    }
}

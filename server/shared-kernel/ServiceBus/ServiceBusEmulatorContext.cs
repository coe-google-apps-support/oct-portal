using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoE.Ideas.Shared.ServiceBus
{
    internal class ServiceBusEmulatorContext : DbContext, IDesignTimeDbContextFactory<ServiceBusEmulatorContext>
    {
        public ServiceBusEmulatorContext() : base() { }

        public ServiceBusEmulatorContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<Message> Messages { get; set; }

        public DbSet<MessageProperty> MessageProperties { get; set; }

        public ServiceBusEmulatorContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ServiceBusEmulatorContext>();
            optionsBuilder.UseSqlServer("server=.;database=ServiceBusEmulator;Trusted_Connection=True;");
            return new ServiceBusEmulatorContext(optionsBuilder.Options);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("server=.;database=ServiceBusEmulator;Trusted_Connection=True;");

            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Message>()
                .Ignore(x => x.MessageProperties);

            base.OnModelCreating(modelBuilder);
        }
    }
}

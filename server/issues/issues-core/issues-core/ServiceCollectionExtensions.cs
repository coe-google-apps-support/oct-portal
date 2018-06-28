using CoE.Ideas.Shared;
using CoE.Issues.Core.Data;
using CoE.Issues.Core.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoE.Issues.Core
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddLocalIssueConfiguration(this IServiceCollection services,
            string dbConnectionString = null,
            string applicationUrl = null,
            ServiceLifetime optionsLifeTime = ServiceLifetime.Scoped)
        {
            // default value is one is not supplied - Note this is not what Production/UAT uses, but just a convenience for local dev
            string connectionString = string.IsNullOrWhiteSpace(dbConnectionString)
                ? "server=wordpress-db;uid=root;pwd=octavadev;database=issues" : dbConnectionString;

            services.AddDbContext<IssueContext>(options =>
                options.UseMySql(connectionString),
                optionsLifetime: optionsLifeTime);

            switch (optionsLifeTime)
            {
                case ServiceLifetime.Scoped:
                    services.AddScoped<IIssueRepository, IssueRepository>();
                    services.AddScoped<IHealthCheckable, IssueRepository>();
                    break;
                case ServiceLifetime.Singleton:
                    services.AddSingleton<IIssueRepository, IssueRepository>();
                    services.AddSingleton<IHealthCheckable, IssueRepository>();
                    break;
                default:
                    services.AddTransient<IIssueRepository, IssueRepository>();
                    services.AddTransient<IHealthCheckable, IssueRepository>();
                    break;
            }

            return services;
        }


#if DEBUG
        public static void InitializeIssueDatabase(this IServiceProvider serviceProvider)
        {
            int retryCount = 0;
            while (true)
            {
                var context = serviceProvider.GetRequiredService<IssueContext>();
                try
                {

                    context.Database.Migrate();
                    break;
                }
                catch (Exception)
                {
                    if (retryCount++ > 30)
                        throw;
                    System.Threading.Thread.Sleep(1000);
                }
            }
        }
#endif

    }
}

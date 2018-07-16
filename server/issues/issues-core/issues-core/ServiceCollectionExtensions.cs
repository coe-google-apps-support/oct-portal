using CoE.Ideas.Shared;
using CoE.Issues.Core.Data;
using CoE.Issues.Core.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using Microsoft.Azure.ServiceBus;

using System.Collections.Generic;
using System.Text;
using Serilog;
using CoE.Issues.Core.ServiceBus;
using System.Linq;
using CoE.Ideas.Shared.People;
using MediatR;
using CoE.Ideas.Shared.Data;

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
                    services.AddScoped<IIssueRepository, LocalIssueRepository>();
                    services.AddScoped<IHealthCheckable, LocalIssueRepository>();
                    break;
                case ServiceLifetime.Singleton:
                    services.AddSingleton<IIssueRepository, LocalIssueRepository>();
                    services.AddSingleton<IHealthCheckable, LocalIssueRepository>();
                    break;
                default:
                    services.AddTransient<IIssueRepository, LocalIssueRepository>();
                    services.AddTransient<IHealthCheckable, LocalIssueRepository>();
                    break;
            }


            services.AddMediatR();
            services.AddScoped<DomainEvents>();
            services.AddScoped<IPersonRepository, PersonRepository>();


            return services;
        }

        public static IServiceCollection AddIssueMessaging(this IServiceCollection services,
            string serviceBusConnectionString = null,
            string serviceBusTopicName = null,
            string serviceBusSubscription = null)
        {
            services.AddSingleton<ITopicClient, TopicClient>(x =>
            {
                return new TopicClient(serviceBusConnectionString, serviceBusTopicName);
            });

            if (string.IsNullOrWhiteSpace(serviceBusConnectionString))
            {
                services.AddScoped<IMessageSender, ServiceBusMessageSender>();
                services.AddScoped<IMessageReceiver, ServiceBusMessageReceiver>();
                services.AddScoped<IIssueMessageSender, IssueMessageSender>();
                // this is how we can determine if we can acces the local database...
                if (services.Any(x => x.ImplementationType == typeof(LocalIssueRepository)))
                    services.AddScoped<IIssueMessageReceiver, LocalIssueMessageReceiver>();
                else
                    services.AddScoped<IIssueMessageReceiver, LocalIssueMessageReceiver>();
            }
            else
            {
                services.AddSingleton<ITopicClient, TopicClient>(x =>
                {
                    return new TopicClient(serviceBusConnectionString, serviceBusTopicName);
                });

                if (!string.IsNullOrWhiteSpace(serviceBusSubscription))
                {
                    services.AddSingleton<ISubscriptionClient, SubscriptionClient>(x =>
                    {
                        return new SubscriptionClient(serviceBusConnectionString, serviceBusTopicName, serviceBusSubscription);
                    });
                }

                services.AddSingleton<IMessageSender, ServiceBusMessageSender>();
                services.AddSingleton<IMessageReceiver, ServiceBusMessageReceiver>();
                services.AddSingleton<IIssueMessageSender, IssueMessageSender>();
                // this is how we can determine if we can acces the local database...
                if (services.Any(x => x.ImplementationType == typeof(LocalIssueRepository)))
                    services.AddSingleton<IIssueMessageReceiver, LocalIssueMessageReceiver>();
                else
                    services.AddSingleton<IIssueMessageReceiver, LocalIssueMessageReceiver>();

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

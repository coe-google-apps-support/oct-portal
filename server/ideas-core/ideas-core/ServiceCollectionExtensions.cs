using CoE.Ideas.Core.Data;
using CoE.Ideas.Core.Events;
using CoE.Ideas.Core.ServiceBus;
using CoE.Ideas.Core.Services;
using CoE.Ideas.Shared;
using CoE.Ideas.Shared.Extensions;
using EnsureThat;
using MediatR;
using Microsoft.Azure.ServiceBus;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace CoE.Ideas.Core
{
    /// <summary>
    /// Extension methods for setting up MVC services in an Microsoft.Extensions.DependencyInjection.IServiceCollection.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds Idea services to the specified Microsoft.Extensions.DependencyInjection.IServiceCollection.
        /// -Specifically, adds an implementation of IIdeaRepository for use with Dependency Injection.
        /// </summary>
        /// <param name="services">The Microsoft.Extensions.DependencyInjection.IServiceCollection to add services to.</param>
        /// <param name="dbConnectionString">The connection string to the Idea database.</param>
        /// <returns>The passed in services, for chaining</returns>
        public static IServiceCollection AddLocalInitiativeConfiguration(this IServiceCollection services,
            string dbConnectionString = null,
            string applicationUrl = null)
        {
            // default value is one is not supplied - Note this is not what Production/UAT uses, but just a convenience for local dev
            string connectionString = string.IsNullOrWhiteSpace(dbConnectionString)
                ? "server=wordpress-db;uid=root;pwd=octavadev;database=initiatives" : dbConnectionString;

            services.AddDbContext<InitiativeContext>(options =>
                options.UseMySql(connectionString));

            services.AddScoped<IInitiativeRepository, LocalInitiativeRepository>();
            services.AddScoped<IHealthCheckable, LocalInitiativeRepository>();

            string applicationUrlFormatted = string.IsNullOrWhiteSpace(applicationUrl)
                ? "http://localhost" : applicationUrl;
            services.AddSingleton<IInitiativeApplicationInfoProvider, InitiativeApplicationInfoProvider>(
                x => new InitiativeApplicationInfoProvider(applicationUrlFormatted));
            services.AddScoped<IInitiativeService, InitiativeService>();

            services.AddMediatR();
            services.AddScoped<DomainEvents>();

            services.AddScoped<IPersonRepository, PersonRepository>();

            services.AddScoped<IStringTemplateService, StringTemplateService>();

            services.AddScoped<IInitiativeStatusEtaRepository, InitiativeStatusEtaRepository>();

            return services;
        }

#if DEBUG
        public static void InitializeInitiativeDatabase(this IServiceProvider serviceProvider)
        {
            int retryCount = 0;
            while (true)
            {
                var context = serviceProvider.GetRequiredService<InitiativeContext>();
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

        public static IServiceCollection AddInitiativeMessaging(this IServiceCollection services,
            SynchronousInitiativeMessageReceiver synchronousInitiativeMessageReceiver)
        {
            services.AddSingleton<SynchronousInitiativeMessageReceiver>(x => synchronousInitiativeMessageReceiver);
            services.AddSingleton<IInitiativeMessageReceiver>(x => x.GetRequiredService<SynchronousInitiativeMessageReceiver>());
            services.AddSingleton<IInitiativeMessageSender, SynchronousInitiativeMessageSender>();
            return services;
        }

        public static IServiceCollection AddInitiativeMessaging(this IServiceCollection services,
            string serviceBusConnectionString = null,
            string serviceBusTopicName = null,
            string serviceBusSubscription = null,
            string serviceBusEmulatorConnectionString = null)
        {
            if (string.IsNullOrWhiteSpace(serviceBusConnectionString))
            {
                services.AddServiceBusEmulator(serviceBusEmulatorConnectionString);
                services.AddScoped<IMessageSender, EmulatedServiceBusMessageSender>();
                services.AddScoped<IMessageReceiver, EmulatedServiceBusMessageReceiver>();
                services.AddScoped<IInitiativeMessageSender, InitiativeMessageSender>();
                // this is how we can determine if we can acces the local database...
                if (services.Any(x => x.ImplementationType == typeof(LocalInitiativeRepository)))
                    services.AddScoped<IInitiativeMessageReceiver, LocalInitiativeMessageReceiver>();
                else
                    services.AddScoped<IInitiativeMessageReceiver, RemoteInitiativeMessageReceiver>();
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
                services.AddSingleton<IInitiativeMessageSender, InitiativeMessageSender>();
                // this is how we can determine if we can acces the local database...
                if (services.Any(x => x.ImplementationType == typeof(LocalInitiativeRepository)))
                    services.AddSingleton<IInitiativeMessageReceiver, LocalInitiativeMessageReceiver>();
                else
                    services.AddSingleton<IInitiativeMessageReceiver, RemoteInitiativeMessageReceiver>();

            }
            return services;
        }

        public static IServiceCollection AddRemoteInitiativeConfiguration(this IServiceCollection services,
            string ideasApiUrl = null,
            string applicationUrl = null)
        {
            if (string.IsNullOrWhiteSpace(ideasApiUrl))
                ideasApiUrl = "http://ideas-server/api";

            services.Configure<RemoteInitiativeRepositoryOptions>(options =>
            {
                options.IdeasApiUrl = ideasApiUrl;
            });
            services.AddTransient<RemoteInitiativeRepository>();
            services.AddTransient<IInitiativeRepository>(x => x.GetRequiredService<RemoteInitiativeRepository>());

            string applicationUrlFormatted = string.IsNullOrWhiteSpace(applicationUrl)
                ? "http://localhost" : applicationUrl;
            services.AddSingleton<IInitiativeApplicationInfoProvider, InitiativeApplicationInfoProvider>(
                x => new InitiativeApplicationInfoProvider(applicationUrlFormatted));
            services.AddScoped<IInitiativeService, InitiativeService>();

            services.Configure<RemoteStatusEtaRepositoryOptions>(x =>
            {
                x.IdeasApiUrl = ideasApiUrl;
            });
            services.AddScoped<IInitiativeStatusEtaRepository, RemoteStatusEtaRepository>();
            services.AddScoped<IInitiativeStatusEtaService, InitiativeStatusEtaService>();

            //services.Configure<WordPressClientOptions>(options =>
            //{
            //    options.Url = new Uri(wordpressUrl);
            //});
            //services.AddScoped<IWordPressClient, WordPressClient>();

            //services.Configure<WordPressUserSecurityOptions>(wordPressConfigurationSection);
            //services.AddSingleton<IWordPressUserSecurity, WordPressUserSecurity>();

            return services;
        }


        public static IServiceCollection AddStatusEtaService(this IServiceCollection services,
            string payrollCalenderServiceUrl = null)
        {
            string calendarServiceUrl = string.IsNullOrWhiteSpace(payrollCalenderServiceUrl)
                 ? "http://webapps1.edmonton.ca/CoE.PayrollCalendar.WebApi/api/PayrollCalendar" : payrollCalenderServiceUrl;
            services.Configure<BusinessCalendarServiceOptions>(x => x.PayrollCalenderServiceUrl = calendarServiceUrl);
            services.AddSingleton<IBusinessCalendarService, BusinessCalendarService>();
            services.AddSingleton<IInitiativeStatusEtaService, InitiativeStatusEtaService>();
            return services;
        }

    }
}

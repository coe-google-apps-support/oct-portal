using CoE.Ideas.Core.Data;
using CoE.Ideas.Core.Events;
using CoE.Ideas.Core.ServiceBus;
using CoE.Ideas.Core.Services;
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
            string dbConnectionString = null)
        { 
            // default value is one is not supplied
            string connectionString = string.IsNullOrWhiteSpace(dbConnectionString)
                ? "server=.;database=CoeIdeas;Trusted_Connection=True;" : dbConnectionString;

            services.AddDbContext<InitiativeContext>(options =>
                options.UseSqlServer(connectionString));

            services.AddScoped<IInitiativeRepository, LocalInitiativeRepository>();

            services.AddMediatR();
            services.AddScoped<DomainEvents>();

            services.AddScoped<IPersonRepository, PersonRepository>();

            services.AddSingleton<IStringTemplateService, StringTemplateService>();

            return services;
        }

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
            string serviceBusSubscription = null)
        {
            if (string.IsNullOrWhiteSpace(serviceBusConnectionString))
            {
                AddInitiativeMessaging(services, new SynchronousInitiativeMessageReceiver());
            }
            else
            {
                services.AddSingleton<ITopicClient, TopicClient>(x =>
                {
                    return new TopicClient(serviceBusConnectionString, serviceBusTopicName);
                });
                services.AddSingleton<IInitiativeMessageSender, InitiativeMessageSender>();

                if (!string.IsNullOrWhiteSpace(serviceBusSubscription))
                {
                    services.AddSingleton<ISubscriptionClient, SubscriptionClient>(x =>
                    {
                        return new SubscriptionClient(serviceBusConnectionString, serviceBusTopicName, serviceBusSubscription);
                    });

                    // this is how we can determine if we can acces the local database...
                    if (services.Any(x => x.ImplementationType == typeof(LocalInitiativeRepository)))
                        services.AddSingleton<IInitiativeMessageReceiver, LocalInitiativeMessageReceiver>();
                    else
                        services.AddSingleton<IInitiativeMessageReceiver, RemoteInitiativeMessageReceiver>();
                }
            }

            return services;
        }

        public static IServiceCollection AddRemoteInitiativeConfiguration(this IServiceCollection services,
            string ideasApiUrl)
        {
            if (string.IsNullOrWhiteSpace(ideasApiUrl))
                throw new ArgumentNullException("ideasApiUrl");

            //services.Configure<RemoteInitiativeRepositoryOptions>(options =>
            //{
            //    options.WordPressUrl = wordpressUrl;
            //});
            services.AddTransient<RemoteInitiativeRepository>();
            services.AddTransient<IInitiativeRepository>(x => x.GetRequiredService<RemoteInitiativeRepository>());


            //services.Configure<WordPressClientOptions>(options =>
            //{
            //    options.Url = new Uri(wordpressUrl);
            //});
            //services.AddScoped<IWordPressClient, WordPressClient>();

            //services.Configure<WordPressUserSecurityOptions>(wordPressConfigurationSection);
            //services.AddSingleton<IWordPressUserSecurity, WordPressUserSecurity>();

            return services;
        }


    }
}

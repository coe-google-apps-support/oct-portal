using CoE.Ideas.Core.Data;
using CoE.Ideas.Core.ServiceBus;
using CoE.Ideas.Core.Services;
using CoE.Ideas.Core.WordPress;
using CoE.Ideas.Shared.Security;
using EnsureThat;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.ServiceBus;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
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
            string dbConnectionString)
        {
            EnsureArg.IsNotNullOrWhiteSpace(dbConnectionString);

            services.AddDbContext<InitiativeContext>(options =>
                options.UseMySql(dbConnectionString));

            services.AddScoped<IInitiativeRepository, LocalInitiativeRepository>();

            //// IHttpContextAccessor is used in WordpressClient
            //services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            //var wordPressUri = new Uri(wordPressUrl);
            //services.Configure<WordPressClientOptions>(options => 
            //{
            //    options.Url = wordPressUri;
            //});
            //services.AddScoped<IWordPressClient, WordPressClient>();

            services.AddSingleton<IStringTemplateService, StringTemplateService>();

            return services;
        }

        public static IServiceCollection AddInitiativeMessaging(this IServiceCollection services,
            string serviceBusConnectionString = null,
            string serviceBusTopicName = null,
            string serviceBusSubscription = null)
        {
            if (string.IsNullOrWhiteSpace(serviceBusConnectionString))
            {
                services.AddSingleton<SynchronousInitiativeMessageReceiver>();
                services.AddSingleton<IInitiativeMessageReceiver>(x => x.GetRequiredService<SynchronousInitiativeMessageReceiver>());
                services.AddSingleton<IInitiativeMessageSender, SynchronousInitiativeMessageSender>();
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
            string ideasApiUrl, 
            string wordpressUrl,
            IConfigurationSection wordPressConfigurationSection)
        {
            if (string.IsNullOrWhiteSpace(ideasApiUrl))
                throw new ArgumentNullException("ideasApiUrl");
            if (string.IsNullOrWhiteSpace(wordpressUrl))
                throw new ArgumentNullException("wordpressUrl");

            services.Configure<RemoteInitiativeRepositoryOptions>(options =>
            {
                options.WordPressUrl = wordpressUrl;
            });
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





        /// <summary>
        /// Adds Idea authentication for WebAPI. Sets up JWT handlers for use with the token generator 
        /// in the WordPress JWT plugin
        /// </summary>
        /// <param name="services">The Microsoft.Extensions.DependencyInjection.IServiceCollection to add services to.</param>
        /// <param name="jwtSecretKey">The JWT_SECRET_KEY, as specified in the corresponding WordPress installation.</param>
        /// <param name="coeAuthKey">The COE_AUTH_KEY, as specified in the corresponding WordPress installation.</param>
        /// <param name="coeAuthIV">The COE_AUTH_IV, as specified in the corresponding WordPress installation.</param>
        /// <param name="wordPressUrl">The full URL of the wordpress installation, used to verify issuers of the JWT token</param>
        /// <returns></returns>
        public static IServiceCollection AddIdeaAuthSecurity(this IServiceCollection services, string wordPressUrl)
        {
            EnsureArg.IsNotNullOrWhiteSpace(wordPressUrl);

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = WordPressCookieAuthenticationDefaults.AuthenticationScheme;
            }).AddWordPressCookie(options =>
            {
                options.WordPressUrl = wordPressUrl;
            });

            return services;
        }
    }
}

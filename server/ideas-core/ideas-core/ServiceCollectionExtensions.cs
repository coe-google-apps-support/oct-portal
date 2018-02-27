using CoE.Ideas.Core.Internal;
using CoE.Ideas.Core.Internal.Initiatives;
using CoE.Ideas.Core.Internal.WordPress;
using CoE.Ideas.Core.People;
using CoE.Ideas.Core.ProjectManagement;
using CoE.Ideas.Core.Security;
using CoE.Ideas.Core.ServiceBus;
using CoE.Ideas.Core.WordPress;
using CoE.Ideas.ProjectManagement.Core.Internal;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.ServiceBus;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

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
        public static IServiceCollection AddIdeaConfiguration(this IServiceCollection services,
            string dbConnectionString, 
            string wordPressUrl,
            string jwtSecretKey = null)
        {
            if (string.IsNullOrWhiteSpace(dbConnectionString))
                throw new ArgumentNullException("dbConnectionString");

            if (string.IsNullOrWhiteSpace(wordPressUrl))
                throw new ArgumentNullException("wordPressUrl");

            services.AddDbContext<IdeaContext>(options =>
                options.UseMySql(dbConnectionString));

            services.AddScoped<IIdeaRepository, IdeaRepositoryInternal>();
            services.AddScoped<IUpdatableIdeaRepository, IdeaRepositoryInternal>();

            // IHttpContextAccessor is used in WordpressClient
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            var wordPressUri = new Uri(wordPressUrl);
            services.Configure<WordPressClientOptions>(options => 
            {
                options.Url = wordPressUri;
            });
            services.AddScoped<IWordPressClient, WordPressClient>();

            return services;
        }

        public static IServiceCollection AddInitiativeMessaging(this IServiceCollection services,
            string serviceBusConnectionString,
            string serviceBusTopicName,
            string serviceBusSubscription = null)
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
                services.AddSingleton<IInitiativeMessageReceiver, InitiativeMessageReceiver>();
            }

            return services;
        }

        public static IServiceCollection AddProjectManagementConfiguration(
            this IServiceCollection services,
            string dbConnectionString)
        {
            if (string.IsNullOrWhiteSpace(dbConnectionString))
                throw new ArgumentNullException("dbConnectionString");


            services.AddDbContext<ProjectManagementContext>(options =>
                options.UseMySql(dbConnectionString));



            services.AddScoped<IProjectManagementRepository, ProjectManagementRepositoryInternal>();

            return services;
        }

        public static IServiceCollection AddRemoteIdeaConfiguration(this IServiceCollection services,
            string ideasApiUrl, string wordpressUrl)
        {
            if (string.IsNullOrWhiteSpace(ideasApiUrl))
                throw new ArgumentNullException("ideasApiUrl");
            if (string.IsNullOrWhiteSpace(wordpressUrl))
                throw new ArgumentNullException("wordpressUrl");


            Uri wordpressUri = new Uri(wordpressUrl);
            services.AddScoped<IIdeaRepository, RemoteIdeaRepository>(x => new RemoteIdeaRepository(ideasApiUrl));


            services.Configure<WordPressClientOptions>(options =>
            {
                options.Url = wordpressUri;
            });
            services.AddScoped<IWordPressClient, WordPressClient>();
            return services;
        }


        public static IServiceCollection AddPeopleService(this IServiceCollection services,
            string peopleServiceUrl)
        {
            if (string.IsNullOrWhiteSpace(peopleServiceUrl))
                throw new ArgumentNullException("peopleServiceUrl");

            Uri peopleServiceUri;
            try
            {
                peopleServiceUri = new Uri(peopleServiceUrl);
            }
            catch (Exception err)
            {
                throw new InvalidOperationException($"peopleServiceUrl is not a valid url: { peopleServiceUrl }", err);
            }

            services.Configure<PeopleServiceOptions>(x =>
            {
                x.ServiceUrl = peopleServiceUri;
            });
            services.AddSingleton<IPeopleService, PeopleService>();

            return services;
        }


        public static IServiceCollection AddWordPressRepository(this IServiceCollection services,
            string dbConnectionString)
        {
            if (string.IsNullOrWhiteSpace(dbConnectionString))
                throw new ArgumentNullException("dbConnectionString");

            services.AddDbContext<WordPressContext>(options =>
                options.UseMySql(dbConnectionString));

            services.AddScoped<IWordPressRepository, WordPressRepository>();

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
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = WordPressCookieAuthenticationDefaults.AuthenticationScheme;
                //options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddWordPressCookie(options =>
            {
                options.WordPressUrl = wordPressUrl;
            });

            return services;
        }

        private class SimpleOptions<T> : IOptions<T> where T : class, new()
        {
            public T Value { get; set; }
        }
    }
}

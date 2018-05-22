using CoE.Ideas.Shared.People;
using CoE.Ideas.Shared.Security;
using CoE.Ideas.Shared.ServiceBus;
using CoE.Ideas.Shared.WordPress;
using EnsureThat;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoE.Ideas.Shared.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddWordPressServices(this IServiceCollection services,
            string wordPressDbConnectionString)
        {
            // defaults (for dev environment) - this is not the same as Production!!
            string connectionString = string.IsNullOrWhiteSpace(wordPressDbConnectionString)
                ? "server=wordpress-db;uid=root;pwd=octavadev;database=OctPortalWordPress"
                : wordPressDbConnectionString;

            services.AddDbContext<WordPressContext>(options =>
                options.UseMySql(connectionString));

            services.AddScoped<IWordPressRepository, WordPressRepository>();
            services.AddScoped<IHealthCheckable, WordPressRepository>();

            return services;

        }


        public static IServiceCollection AddWordPressSecurity(this IServiceCollection services,
            IConfigurationSection wordPressConfigurationSection
#if DEBUG
            ,string staticDevUserName = null
            ,string staticDevEmail = null
#endif
            )
        {
            // defaults (for dev environment)
            string wordPressUrl = string.IsNullOrWhiteSpace(wordPressConfigurationSection["Url"])
                ? "http://localhost"
                : wordPressConfigurationSection["Url"];
#if DEBUG

            // NOTE this our not the keys we use in INT/UAT/Production !!
            if (string.IsNullOrWhiteSpace(wordPressConfigurationSection["Url"])) wordPressConfigurationSection["Url"] = wordPressUrl;
            if (string.IsNullOrWhiteSpace(wordPressConfigurationSection["AUTH_KEY"])) wordPressConfigurationSection["AUTH_KEY"] = "df66691c29f5c411518e34b63a1596e7b4c8c592f7374f2aa31d18b37a6a706b";
            if (string.IsNullOrWhiteSpace(wordPressConfigurationSection["SECURE_AUTH_KEY"])) wordPressConfigurationSection["SECURE_AUTH_KEY"] = "20833cf1548f33907dc1d22594bc327090fcf36658aa99cc3893d2ac83c60bdc";
            if (string.IsNullOrWhiteSpace(wordPressConfigurationSection["LOGGED_IN_KEY"])) wordPressConfigurationSection["LOGGED_IN_KEY"] = "e062894f6f210de789922a1610163bd0ecc4b37dcc9586a90b42d6de55edc3a1";
            if (string.IsNullOrWhiteSpace(wordPressConfigurationSection["NONCE_KEY"])) wordPressConfigurationSection["NONCE_KEY"] = "72162ab50ed34a99d2e968302a2e79e71dd5b7174472b6355cb1fe2f70aec36a";
            if (string.IsNullOrWhiteSpace(wordPressConfigurationSection["AUTH_SALT"])) wordPressConfigurationSection["AUTH_SALT"] = "cc7d75e599261db4f3e3dc08055fc324cd1f4764a7136d0d2b41cf66cd4feb4e";
            if (string.IsNullOrWhiteSpace(wordPressConfigurationSection["SECURE_AUTH_SALT"])) wordPressConfigurationSection["SECURE_AUTH_SALT"] = "633b9c2b2eaaba702c5fb1130951cdaadd8a1940504c7f85879453776cc59fe0";
            if (string.IsNullOrWhiteSpace(wordPressConfigurationSection["LOGGED_IN_SALT"])) wordPressConfigurationSection["LOGGED_IN_SALT"] = "2cb69d64dd4a85b634eaf26b8e77b0fa18f430591c2f573485a370f6ed8e4424";
            if (string.IsNullOrWhiteSpace(wordPressConfigurationSection["NONCE_SALT"])) wordPressConfigurationSection["NONCE_SALT"] = "c9e9e4dcccf9fb7dc5daf5275ce88f1aef33bc031a558a9845678c741fdfdf92";

            if (string.IsNullOrWhiteSpace(wordPressConfigurationSection["AdminServicePrincipalName"])) wordPressConfigurationSection["AdminServicePrincipalName"] = "octavadev";
            if (string.IsNullOrWhiteSpace(wordPressConfigurationSection["AdminServicePrincipalPassword"])) wordPressConfigurationSection["AdminServicePrincipalPassword"] = "password";

#endif

            // sometimes IHTtpContextAccessor is not wired up by default, so ensure that it is
            services.TryAddSingleton<Microsoft.AspNetCore.Http.IHttpContextAccessor, Microsoft.AspNetCore.Http.HttpContextAccessor>();

            services.Configure<WordPressUserSecurityOptions>(wordPressConfigurationSection);
            services.AddScoped<IWordPressUserSecurity, WordPressUserSecurity>();

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = WordPressCookieAuthenticationDefaults.AuthenticationScheme;
            }).AddWordPressCookie(options =>
            {
                options.WordPressUrl = wordPressUrl;
#if DEBUG
                if (!string.IsNullOrWhiteSpace(staticDevUserName) && !string.IsNullOrWhiteSpace(staticDevEmail))
                {
                    options.DevUserName = staticDevUserName;
                    options.DevUserEmail = staticDevEmail;
                }
#endif
            });

            return services;
        }

        public static IServiceCollection AddPermissionSecurity(this IServiceCollection services,
            string permissionDbConnectionString)
        {
            // defaults (for dev environment) - this is not the same as Production!!
            string connectionString = string.IsNullOrWhiteSpace(permissionDbConnectionString)
                ? "server=wordpress-db;uid=root;pwd=octavadev;database=OctPortalWordPress"
                : permissionDbConnectionString;

            services.AddDbContext<SecurityContext>(options =>
                options.UseSqlServer(connectionString));

            return services;
        }

        public static IServiceCollection AddPeopleService(this IServiceCollection services,
            string peopleServiceUrl = null)
        {
            // default value
            string url = string.IsNullOrWhiteSpace(peopleServiceUrl)
                ? "http://webapps.edmonton.ca/CoE.PeopleDirectory.WebApi/api/PeopleDirectory" 
                : peopleServiceUrl;

            Uri peopleServiceUri;
            try
            {
                peopleServiceUri = new Uri(url);
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

            services.AddMemoryCache();

            return services;
        }

        public static IServiceCollection AddServiceBusEmulator(this IServiceCollection services,
            string serviceBusConnectionString = null)
        {
            string connectionString = string.IsNullOrWhiteSpace(serviceBusConnectionString)
                ? "server=initiatives-db;database=ServiceBusEmulator;User Id=SA;Password=OctavaDev100!;MultipleActiveResultSets=True;"
                : serviceBusConnectionString;

            services.AddDbContext<ServiceBusEmulatorContext>(options => options.UseSqlServer(connectionString));
            services.AddScoped<IServiceBusEmulator, ServiceBusEmulator>();

            return services;
        }

#if DEBUG
        public static void InitiativeServiceBusEmlatorDatabase(this IServiceProvider serviceProvider)
        {
            int retryCount = 0;
            while (true)
            {
                var context = serviceProvider.GetRequiredService<ServiceBusEmulatorContext>();
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

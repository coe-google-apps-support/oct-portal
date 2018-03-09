using CoE.Ideas.Shared.People;
using CoE.Ideas.Shared.WordPress;
using EnsureThat;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoE.Ideas.Shared.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddWordPressSecurity(this IServiceCollection services,
            string wordPressDbConnectionString,
            IConfigurationSection wordPressConfigurationSection)
        {
            EnsureArg.IsNotNullOrWhiteSpace(wordPressDbConnectionString);
            EnsureArg.IsNotNull(wordPressConfigurationSection);

            services.AddDbContext<WordPressContext>(options =>
                options.UseMySql(wordPressDbConnectionString));

            services.Configure<WordPressUserSecurityOptions>(wordPressConfigurationSection);
            services.AddScoped<IWordPressUserSecurity, WordPressUserSecurity>();

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
    }
}

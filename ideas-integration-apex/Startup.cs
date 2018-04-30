using System;
using CoE.Ideas.Core;
using CoE.Ideas.Server.Controllers;
using CoE.Ideas.Shared.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

namespace CoE.Ideas.Integration.Apex
{
    internal class Startup
    {
        private IConfigurationRoot Configuration;

        public Startup(IConfigurationRoot configuration)
        {
            Configuration = configuration;
            var services = ConfigureServices(new ServiceCollection());

            var serviceProvider = services.BuildServiceProvider();

            // instantiate the NewIdeaListener at least once to start the message pump
            serviceProvider.GetRequiredService<ApexListener>();
        }

        private IServiceCollection ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();

            // Add logging
            services.AddSingleton(new LoggerFactory()
                .AddConsole(Configuration)
                .AddDebug((str, logLevel) =>
                {
                    // Microsoft is too noisy!
                    //                    return logLevel >= LogLevel.Warning || (!string.IsNullOrWhiteSpace(str) && !str.StartsWith("Microsoft"));
                    if (logLevel >= LogLevel.Warning)
                        return true;
                    else if (string.IsNullOrWhiteSpace(str))
                        return false;
                    else if (str.StartsWith("Microsoft"))
                        return false;
                    else
                        return true;
                })
                .AddSerilog());
            services.AddLogging();

            // configure application specific logging
            services.AddSingleton<Serilog.ILogger>(x => new LoggerConfiguration()
                .Enrich.FromLogContext()
                .Enrich.WithProperty("Application", "Initiatives")
                .Enrich.WithProperty("Module", "Apex Listener")
                .ReadFrom.Configuration(Configuration)
                .CreateLogger());


            // Add Idea Repository
            services.AddLocalInitiativeConfiguration(
                Configuration.GetConnectionString("IdeaDatabase"));


            services.AddWordPressServices(Configuration.GetConnectionString("WordPressDatabase"));

            services.AddPeopleService();

            services.Configure<ApexOptions>(x => x.ApexConnectionString = Configuration.GetConnectionString("Apex"));

            services.AddSingleton<ApexListener>();

            services.AddScoped<IdeasController>();

            services.AddWordPressSecurity(Configuration.GetSection("WordPress"));

            services.AddInitiativeMessaging(serviceBusEmulatorConnectionString: Configuration.GetConnectionString("ServiceBusEmulator"));

            return services;
        }
    }
}
﻿using System;
using System.Threading.Tasks;
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
            var listener = serviceProvider.GetRequiredService<ApexListener>();
            Task.Run(() => listener.Read()).Wait();
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
            services.ConfigureLogging(Configuration, "Apex Listener");

            // Add Idea Repository
            services.AddLocalInitiativeConfiguration(
                Configuration.GetConnectionString("IdeaDatabase"));


            services.AddWordPressServices(Configuration.GetConnectionString("WordPressDatabase"));

            services.AddPeopleService();

            services.Configure<ApexOptions>(x => x.ApexConnectionString = Configuration.GetConnectionString("Apex"));

            services.AddSingleton<ApexListener>();

            services.AddScoped<IdeasController>();

            services.AddWordPressSecurity(Configuration.GetSection("WordPress"));

            services.AddInitiativeMessaging(Configuration.GetConnectionString("IdeaServiceBus"),
                Configuration["Ideas:ServiceBusTopic"],
                serviceBusEmulatorConnectionString: Configuration.GetConnectionString("ServiceBusEmulator"));

            return services;
        }
    }
}
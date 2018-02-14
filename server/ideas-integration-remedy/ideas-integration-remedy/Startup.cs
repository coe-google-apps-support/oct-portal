﻿using CoE.Ideas.Core;
using CoE.Ideas.Core.ServiceBus;
using CoE.Ideas.Core.WordPress;
using CoE.Ideas.Remedy.RemedyServiceReference;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Text;

namespace CoE.Ideas.Remedy
{
    public class Startup
    {
        public Startup(IConfigurationRoot configuration)
        {
            Configuration = configuration;
            var services = ConfigureServices(new ServiceCollection());

            var serviceProvider = services.BuildServiceProvider();

            // instantiate the NewIdeaListener at least once to start the message pump
            serviceProvider.GetRequiredService<NewIdeaListener>();
        }

        private readonly IConfigurationRoot Configuration;

        private IServiceCollection ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();         

            // Add logging
            services.AddSingleton(new LoggerFactory()
                .AddConsole(Configuration)
                .AddDebug()
                .AddSerilog());
            services.AddLogging();

            // configure application specific logging
            services.AddSingleton<Serilog.ILogger>(x => new LoggerConfiguration()
                .Enrich.FromLogContext()
                .Enrich.WithProperty("Application", "Initiatives")
                .Enrich.WithProperty("Module", "Remedy WO Creator")
                .ReadFrom.Configuration(Configuration)
                .CreateLogger());

            services.AddRemoteIdeaConfiguration(Configuration["IdeasApi"],
                Configuration["WordPressUrl"]);


            services.AddInitiativeMessaging(Configuration["ServiceBus:ConnectionString"],
                Configuration["ServiceBus:TopicName"],
                Configuration["ServiceBus:Subscription"]);

            //services.AddSingleton<IActiveDirectoryUserService, ActiveDirectoryUserService>(x =>
            //{
            //    return new ActiveDirectoryUserService(
            //        Configuration["ActiveDirectory:Domain"],
            //        Configuration["ActiveDirectory:ServiceUserName"],
            //        Configuration["ActiveDirectory:ServicePassword"]);
            //});

            services.AddSingleton(x =>
            {
                return new New_Port_0PortTypeClient(
                    new BasicHttpBinding(BasicHttpSecurityMode.None),
                    new EndpointAddress(Configuration["Remedy:ApiUrl"]));
            });
            services.Configure<RemedyServiceOptions>(Configuration.GetSection("Remedy"));
            services.AddSingleton<IRemedyService, RemedyService>();

            return services;
        }
    }
}

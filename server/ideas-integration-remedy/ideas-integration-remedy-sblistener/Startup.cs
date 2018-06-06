using AutoMapper;
using CoE.Ideas.Core;
using CoE.Ideas.Core.ServiceBus;
using CoE.Ideas.Shared.Extensions;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoE.Ideas.Remedy.SbListener
{
    public class Startup
    {
        public Startup(IConfigurationRoot configuration)
        {
            Configuration = configuration;
            var services = ConfigureServices(new ServiceCollection());

            var serviceProvider = services.BuildServiceProvider();

            // sleep for 30s to ensure are required services are up and running
            // (most affects local development in Docker but is fine for production)
            System.Threading.Thread.Sleep(30); 

            // TODO: eliminate the need to ask for IIdeaServiceBusReceiver to make sure we're listening
            var listener = serviceProvider.GetRequiredService<RemedyItemUpdatedIdeaListener>();
            
        }

        private readonly IConfigurationRoot Configuration;

        private IServiceCollection ConfigureServices(IServiceCollection services)
        {
            // basic stuff - there's probably a better way to register these
            services.AddSingleton(
                typeof(Microsoft.Extensions.Options.IOptions<>),
                typeof(Microsoft.Extensions.Options.OptionsManager<>));
            services.AddSingleton(
                typeof(Microsoft.Extensions.Options.IOptionsFactory<>),
                typeof(Microsoft.Extensions.Options.OptionsFactory<>));

            // Add logging
            services.AddSingleton(new LoggerFactory()
                .AddConsole(Configuration)
                .AddDebug()
                .AddSerilog());
            services.AddLogging();

            // configure application specific logging
            services.ConfigureLogging(Configuration, "Remedy Service Bus Listener");

            // Add Idea Repository
            services.AddLocalInitiativeConfiguration(
                Configuration.GetConnectionString("IdeaDatabase"));


            services.AddWordPressServices(Configuration.GetConnectionString("WordPressDatabase"));

            // Add service to talk to ServiceBus
            services.AddInitiativeMessaging(Configuration["ServiceBus:ConnectionString"],
                    Configuration["ServiceBus:TopicName"],
                    Configuration["ServiceBus:Subscription"]);

            services.AddSingleton<RemedyItemUpdatedIdeaListener>();

            services.AddAutoMapper();

            return services;
        }
    }
}

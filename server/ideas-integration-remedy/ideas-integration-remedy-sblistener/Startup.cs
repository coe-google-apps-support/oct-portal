using AutoMapper;
using CoE.Ideas.Core;
using CoE.Ideas.Core.ServiceBus;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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
                .AddConsole(
                    Enum.Parse<LogLevel>(Configuration["Logging:Debug:LogLevel:Default"]),
                    bool.Parse(Configuration["Logging:IncludeScopes"]))
                .AddDebug(
                    Enum.Parse<LogLevel>(Configuration["Logging:Console:LogLevel:Default"])));
            services.AddLogging();

            // Add Idea Repository
            services.AddIdeaConfiguration(
                Configuration.GetConnectionString("IdeaDatabase"),
                Configuration["Ideas:WordPressUrl"],
                Configuration.GetConnectionString("IdeaServiceBus"),
                Configuration["Ideas:ServiceBusTopic"]);

            // Add service to talk to ServiceBus
            services.AddSingleton<ISubscriptionClient>(x =>
            {
                return new SubscriptionClient(Configuration["ServiceBus:ConnectionString"],
                    Configuration["ServiceBus:TopicName"],
                    Configuration["ServiceBus:Subscription"]);
            });

            services.AddSingleton<RemedyItemUpdatedIdeaListener>();

            services.AddAutoMapper();

            return services;
        }
    }
}

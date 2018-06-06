﻿using CoE.Ideas.Core;
using CoE.Ideas.Shared.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

namespace CoE.Ideas.Webhooks
{
    public class Startup
    {
        public Startup(IConfigurationRoot configuration)
        {
            Configuration = configuration;
            var services = ConfigureServices(new ServiceCollection());

            var serviceProvider = services.BuildServiceProvider();

            // Ensure NewIdeaListener is created to start the message pump (in it's constructor)
            serviceProvider.GetRequiredService<WebhookPoster>();
        }

        private readonly IConfigurationRoot Configuration;

        private IServiceCollection ConfigureServices(IServiceCollection services)
        {
            // basic stuff
            services.AddOptions();

            // Add logging 
            services.AddSingleton(new LoggerFactory()
                .AddConsole(Configuration)
                .AddDebug()
                .AddSerilog());
            services.AddLogging();

            // configure application specific logging
            services.ConfigureLogging(Configuration, "Webhooks");

            services.AddRemoteInitiativeConfiguration(Configuration["IdeasApi"]);

            services.AddWordPressSecurity(Configuration.GetSection("WordPress"));
            services.AddPeopleService();

            services.AddInitiativeMessaging(Configuration["ServiceBus:ConnectionString"],
                Configuration["ServiceBus:TopicName"],
                Configuration["ServiceBus:Subscription"]);

            services.Configure<WebhookUrlServiceOptions>(Configuration.GetSection("Webhooks"));

            services.AddSingleton<WebhookPoster>();
            services.AddSingleton<IWebhookUrlService, WebhookUrlService>();
            return services;
        }
    }
}

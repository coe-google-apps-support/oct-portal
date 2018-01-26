using AutoMapper;
using CoE.Ideas.Core;
using CoE.Ideas.Core.ServiceBus;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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

            services.AddIdeaConfiguration(
                Configuration.GetConnectionString("IdeaDatabase"),
                Configuration["Ideas:WordPressUrl"],
                Configuration.GetConnectionString("IdeaServiceBus"),
                Configuration["Ideas:ServiceBusTopic"]);

            //services.AddIdeaListener<RemedyItemUpdatedIdeaListener>(
            //    Configuration["ServiceBus:ConnectionString"],
            //    Configuration["ServiceBus:TopicName"],
            //    Configuration["ServiceBus:Subscription"]);

            services.AddSingleton<ISubscriptionClient>(x =>
            {
                return new SubscriptionClient(Configuration["ServiceBus:ConnectionString"],
                    Configuration["ServiceBus:TopicName"],
                    Configuration["ServiceBus:Subscription"]);
            });
            services.AddSingleton(x =>
            {
                var ideaRepository = x.GetRequiredService<IIdeaRepository>();
                var subscriptionClient = x.GetRequiredService<ISubscriptionClient>();
                return new RemedyItemUpdatedIdeaListener(ideaRepository, subscriptionClient);
            });

            services.AddAutoMapper();

            return services;
        }
    }
}

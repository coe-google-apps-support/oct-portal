using CoE.Ideas.Core;
using CoE.Ideas.Core.ServiceBus;
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
            serviceProvider.GetRequiredService<IIdeaServiceBusReceiver>();
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

            services.AddRemoteIdeaConfiguration(Configuration["IdeasApi"],
                Configuration["WordPressUrl"]);
            services.AddIdeaListener<RemedyItemUpdatedIdeaListener>(
                Configuration["ServiceBus:ConnectionString"],
                Configuration["ServiceBus:TopicName"],
                Configuration["ServiceBus:Subscription"]);

            return services;
        }
    }
}

using CoE.Ideas.Core;
using CoE.Ideas.Core.ServiceBus;
using CoE.Ideas.Core.WordPress;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RemedyServiceReference;
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


            // Add logging
            services.AddSingleton(new LoggerFactory()
                .AddConsole(
                    Enum.Parse<LogLevel>(Configuration["Logging:Debug:LogLevel:Default"]),
                    bool.Parse(Configuration["Logging:IncludeScopes"]))
                .AddDebug(
                    Enum.Parse<LogLevel>(Configuration["Logging:Console:LogLevel:Default"])));
            services.AddLogging();


            services.AddRemoteIdeaConfiguration(Configuration["IdeasApi"],
                Configuration["WordPressUrl"]);
            services.AddIdeaListener<NewIdeaListener>(
                Configuration["ServiceBus:ConnectionString"],
                Configuration["ServiceBus:TopicName"],
                Configuration["ServiceBus:Subscription"], x =>
                {
                    return new NewIdeaListener(x.GetRequiredService<IIdeaRepository>(),
                        x.GetRequiredService<IWordPressClient>(),
                        x.GetRequiredService<IRemedyService>(),
                        new Microsoft.Azure.ServiceBus.TopicClient(Configuration["ServiceBus:ConnectionString"], Configuration["ServiceBus:TopicName"]),
                        x.GetRequiredService<Microsoft.Extensions.Logging.ILogger<NewIdeaListener>>());
                });
            //services.AddSingleton<IActiveDirectoryUserService, ActiveDirectoryUserService>(x =>
            //{
            //    return new ActiveDirectoryUserService(
            //        Configuration["ActiveDirectory:Domain"],
            //        Configuration["ActiveDirectory:ServiceUserName"],
            //        Configuration["ActiveDirectory:ServicePassword"]);
            //});
            services.AddIdeaServiceBusSender(
                Configuration["ServiceBus:ConnectionString"],
                Configuration["ServiceBus:TopicName"]);

            services.AddSingleton(x =>
            {
                return new New_Port_0PortTypeClient(
                    new BasicHttpBinding(BasicHttpSecurityMode.None),
                    new EndpointAddress(Configuration["Remedy:ApiUrl"]));
            });
            services.Configure<RemedyServiceOptions>(options =>
            {
                options.CategorizationTier1 = Configuration["Remedy:CategorizationTier1"];
                options.CategorizationTier2 = Configuration["Remedy:CategorizationTier2"];
                options.LocationCompany = Configuration["Remedy:LocationCompany"];
                options.CustomerCompany = Configuration["Remedy:CustomerCompany"];
                options.ServicePassword = Configuration["Remedy:ServicePassword"];
                options.ServiceUserName = Configuration["Remedy:ServiceUserName"];
                options.TemplateId = Configuration["Remedy:TemplateId"];
                options.CustomerLoginId = Configuration["Remedy:CustomerLoginId"];
                options.CustomerFirstName = Configuration["Remedy:CustomerFirstName"];
                options.CustomerLastName = Configuration["Remedy:CustomerLastName"];
            });
            services.AddSingleton<IRemedyService, RemedyService>();

            return services;
        }
    }
}

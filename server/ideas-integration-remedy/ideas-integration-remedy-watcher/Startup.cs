using CoE.Ideas.Core;
using CoE.Ideas.Core.ServiceBus;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RemedyServiceReference;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace CoE.Ideas.Remedy.Watcher
{
    public class Startup
    {
        public Startup(IConfigurationRoot configuration)
        {
            Configuration = configuration;
            var services = ConfigureServices(new ServiceCollection());

            ServiceProvider = services.BuildServiceProvider();

        }


        private readonly IConfigurationRoot Configuration;
        private readonly IServiceProvider ServiceProvider;

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

            // Add service to talk to ServiceBus
            services.AddSingleton<ITopicClient>(x =>
            {
                return new TopicClient(Configuration.GetConnectionString("ServiceBus"), Configuration["Ideas:ServiceBusTopic"]);
            });

            // Add services to talk to Remedy
            services.Configure<RemedyCheckerOptions>(options =>
            {
                options.ServiceUserName = Configuration["Remedy:ServiceUserName"];
                options.ServicePassword = Configuration["Remedy:ServicePassword"];
                options.TemplateName = Configuration["Remedy:TemplateName"];
                options.ApiUrl = Configuration["Remedy:ApiUrl"];
                options.TempDirectory = Configuration["Remedy:TempDirectory"];
            });
            services.AddSingleton<New_Port_0PortType, 
                New_Port_0PortTypeClient>(x => 
                    new New_Port_0PortTypeClient(new BasicHttpBinding(BasicHttpSecurityMode.None)
                    {
                        MaxReceivedMessageSize = 16777216L // 16 MB, default it 65kb
                    },
                    new EndpointAddress(Configuration["Remedy:ApiUrl"])));
            services.AddSingleton<RemedyChecker>();

            return services;
        }

        public async Task Start()
        {
            var checker = ServiceProvider.GetRequiredService<RemedyChecker>();
            await checker.Poll();
        }

    }
}

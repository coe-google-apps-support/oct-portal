using CoE.Ideas.Core;
using CoE.Ideas.Core.ServiceBus;
using CoE.Ideas.Remedy.Watcher.RemedyServiceReference;
using CoE.Ideas.Shared.Extensions;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
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
            services.AddOptions();

            // Add logging
            services.AddSingleton(new LoggerFactory()
                .AddConsole(Configuration)
                .AddDebug()
                .AddSerilog());
            services.AddLogging();

            // configure application specific logging
#if DEBUG
            services.ConfigureLogging(Configuration, "Remedy WO Creator", useSqlServer: true);
#else
            services.ConfigureLogging(Configuration, "Remedy WO Creator");
#endif

            services.AddRemoteInitiativeConfiguration(Configuration["IdeasApi"],
                Configuration["WordPress:Url"]);


            // Add service to talk to ServiceBus
            services.AddInitiativeMessaging(Configuration.GetConnectionString("ServiceBus"), 
                Configuration["Ideas:ServiceBusTopic"]);

            services.AddWordPressSecurity(Configuration.GetSection("WordPress"));

            // Add services to talk to Remedy
            services.Configure<RemedyCheckerOptions>(Configuration.GetSection("Remedy"));
            services.AddSingleton<New_Port_0PortType, 
                New_Port_0PortTypeClient>(x => 
                    new New_Port_0PortTypeClient(new BasicHttpBinding(BasicHttpSecurityMode.None)
                    {
                        MaxReceivedMessageSize = 16777216L // 16 MB, default it 65kb
                    },
                    new EndpointAddress(Configuration["Remedy:ApiUrl"])));
            services.AddSingleton<RemedyChecker>();

            services.AddSingleton<IRemedyService, RemedyService>();

            services.AddPeopleService(Configuration["PeopleService"]);

            services.AddStatusEtaService();

            return services;
        }

        public async Task Start()
        {
            var checker = ServiceProvider.GetRequiredService<RemedyChecker>();
            await checker.Poll();
        }

    }
}

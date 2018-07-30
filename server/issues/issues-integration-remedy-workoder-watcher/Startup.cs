using AutoMapper;
using CoE.Ideas.Shared.Extensions;
using CoE.Issues.Core;
using CoE.Issues.Remedy.Watcher.RemedyServiceReference;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.ServiceModel;
using System.Threading.Tasks;

namespace CoE.Issues.Remedy.WorkOrder.Watcher
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
            services.ConfigureLogging(Configuration, "Remedy WorkOrder Watcher", useSqlServer: true);
#else
            services.ConfigureLogging(Configuration, "Remedy WO Watcher");
#endif

            // Add services to talk to Remedy
            services.Configure<RemedyCheckerOptions>(Configuration.GetSection("Remedy"));
            services.AddSingleton<New_Port_0PortType,
                New_Port_0PortTypeClient>(x =>
                {
                    var returnValue = new New_Port_0PortTypeClient(new BasicHttpBinding(BasicHttpSecurityMode.None)
                    {
                        MaxReceivedMessageSize = 1073741824L, // 1 GB, default it 65kb
                        ReceiveTimeout = TimeSpan.FromMinutes(10),
                        SendTimeout = TimeSpan.FromMinutes(10)
                    },
                        new EndpointAddress(Configuration["Remedy:ApiUrl"]));
                    services.EnableWcfLogging(returnValue.Endpoint, x.GetRequiredService<Serilog.ILogger>());
                    return returnValue;
                });
            services.AddSingleton<IRemedyChecker, RemedyChecker>();

            services.AddSingleton<IRemedyService, RemedyService>();

            // Add services to talk to ServiceBus
            services.AddIssueMessaging(Configuration["ServiceBus:ConnectionString"],
                Configuration["ServiceBus:TopicName"],
                Configuration["ServiceBus:Subscription"]);

            // configure automapper
            services.AddAutoMapper();

            services.AddPeopleService(Configuration["PeopleService"]);

            return services;
        }

        public async Task Start()
        {
            var checker = ServiceProvider.GetRequiredService<IRemedyChecker>();
            await checker.Poll();
        }

    }
}

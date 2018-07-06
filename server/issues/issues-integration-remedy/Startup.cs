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

namespace CoE.Issues.Remedy
{
    public class Startup
    {

        public Startup(IConfigurationRoot configuration)
        {
            Configuration = configuration;
            var services = ConfigureServices(new ServiceCollection());

            var ServiceProvider = services.BuildServiceProvider();

            // instantiate the NewIdeaListener at least once to start the message pump
            // serviceProvider.GetRequiredService<NewIdeaListener>();

        }

        private readonly IConfigurationRoot Configuration;

        private IServiceCollection ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();

            // Add logging
            services.AddSingleton(new LoggerFactory()
                .AddConsole(Configuration)
                .AddDebug((str, logLevel) =>
                {
                    // Microsoft is too noisy!
                    //                    return logLevel >= LogLevel.Warning || (!string.IsNullOrWhiteSpace(str) && !str.StartsWith("Microsoft"));
                    if (logLevel >= LogLevel.Warning)
                        return true;
                    else if (string.IsNullOrWhiteSpace(str))
                        return false;
                    else if (str.StartsWith("Microsoft"))
                        return false;
                    else
                        return true;
                })
                .AddSerilog());
            services.AddLogging();

            // configure application specific logging
#if DEBUG
            services.ConfigureLogging(Configuration, "Remedy WO Creator", useSqlServer: true);
#else
            services.ConfigureLogging(Configuration, "Remedy WO Creator");
#endif



            services.AddIssueMessaging(Configuration["ServiceBus:ConnectionString"],
                Configuration["ServiceBus:TopicName"],
                Configuration["ServiceBus:Subscription"]);

            services.AddWordPressSecurity(Configuration.GetSection("WordPress"));

            services.AddPeopleService();


            services.AddSingleton<New_Port_0PortType>(x =>
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

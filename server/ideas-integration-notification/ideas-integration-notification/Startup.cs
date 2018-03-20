using CoE.Ideas.Core;
using CoE.Ideas.Core.ServiceBus;
using Microsoft.AspNetCore.NodeServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using System.IO;

namespace CoE.Ideas.Integration.Notification
{
    public class Startup
    {
        public Startup(IConfigurationRoot configuration)
        {
            Configuration = configuration;
            var services = ConfigureServices(new ServiceCollection());

            var serviceProvider = services.BuildServiceProvider();

            // ensure the listener is created at least once so that the message pump 
            // 
            serviceProvider.GetRequiredService<IdeaLoggedListener>();
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
            services.AddSingleton<Serilog.ILogger>(x => new LoggerConfiguration()
                .Enrich.FromLogContext()
                .Enrich.WithProperty("Application", "Initiatives")
                .Enrich.WithProperty("Module", "Notifications")
                .ReadFrom.Configuration(Configuration)
                .CreateLogger());

            services.AddRemoteInitiativeConfiguration(Configuration["IdeasApi"]);
            services.AddInitiativeMessaging(Configuration["ServiceBus:ConnectionString"],
                Configuration["ServiceBus:TopicName"],
                Configuration["ServiceBus:Subscription"]);
            services.AddSingleton( x =>
                new IdeaLoggedListener(
                    x.GetRequiredService<IMailmanEnabledSheetReader>(),
                    x.GetRequiredService<IEmailService>(),
                    x.GetRequiredService<IInitiativeMessageReceiver>(),
                    x.GetRequiredService<Serilog.ILogger>(),
                    Configuration["Notification:MergeTemplate"])
                );

            services.AddSingleton<IEmailService, EmailService>(x =>
            {
                return new EmailService(
                    x.GetRequiredService<INodeServices>(),
                    Configuration["Email:Smtp"],
                    Configuration["Email:FromAddress"],
                    Configuration["Email:FromDisplayName"]);
            });
            services.AddSingleton<IMailmanEnabledSheetReader, MailmanEnabledSheetReader>(
                x =>
                {
                    return new MailmanEnabledSheetReader(
                        Configuration["Notification:serviceAccountPrivateKey"],
                        Configuration["Notification:serviceAccountEmail"],
                        Configuration["Notification:spreadsheetId"]
                    );
                });

            services.AddNodeServices(x =>
            {
                x.ProjectPath = Directory.GetCurrentDirectory();
            });

            return services;
        }
    }
}

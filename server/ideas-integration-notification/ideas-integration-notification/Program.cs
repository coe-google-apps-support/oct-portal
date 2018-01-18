using CoE.Ideas.Core;
using CoE.Ideas.Core.ServiceBus;
using CoE.Ideas.Core.WordPress;
using Microsoft.AspNetCore.NodeServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Threading;

namespace CoE.Ideas.Integration.Notification
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = GetConfig();

            var services = new ServiceCollection();

            // basic stuff - there's probably a better way to register these
            services.AddSingleton(
                typeof(Microsoft.Extensions.Options.IOptions<>),
                typeof(Microsoft.Extensions.Options.OptionsManager<>));
            services.AddSingleton(
                typeof(Microsoft.Extensions.Options.IOptionsFactory<>),
                typeof(Microsoft.Extensions.Options.OptionsFactory<>));

            services.AddRemoteIdeaConfiguration(config["IdeasApi"], 
                config["WordPressUrl"]);
            services.AddIdeaListener<IdeaLoggedListener>(
                config["ServiceBus:ConnectionString"],
                config["ServiceBus:TopicName"],
                config["ServiceBus:Subscription"], x =>
                {
                    return new IdeaLoggedListener(
                        x.GetRequiredService<IIdeaRepository>(),
                        x.GetRequiredService<IWordPressClient>(),
                        x.GetRequiredService<IMailmanEnabledSheetReader>(),
                        x.GetRequiredService<IEmailService>(),
                        config["Notification:MergeTemplate"]);
                });

            services.AddSingleton<IEmailService, EmailService>(x =>
            {
                return new EmailService(
                    x.GetRequiredService<INodeServices>(),
                    config["Email:Smtp"],
                    config["Email:FromAddress"],
                    config["Email:FromDisplayName"]);
            });
            services.AddSingleton<IMailmanEnabledSheetReader, MailmanEnabledSheetReader>(
                x =>
                {
                    return new MailmanEnabledSheetReader(
                        config["Notification:serviceAccountPrivateKey"],
                        config["Notification:serviceAccountEmail"],
                        config["Notification:spreadsheetId"]
                    );
                });

            services.AddNodeServices(x =>
            {
                x.ProjectPath = Directory.GetCurrentDirectory();
            });

            var serviceProvider = services.BuildServiceProvider();

            // TODO: eliminate the need to ask for IIdeaServiceBusReceiver to make sure we're listening
            serviceProvider.GetRequiredService<IIdeaServiceBusReceiver>();

            // now block forever
            // but I don't think the code will ever get here anyway...
            new ManualResetEvent(false).WaitOne();
        }

        private static IConfigurationRoot GetConfig()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true)
                .AddEnvironmentVariables();

            return builder.Build();
        }

    }
}

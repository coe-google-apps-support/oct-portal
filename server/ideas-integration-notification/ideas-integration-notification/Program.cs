using CoE.Ideas.Core;
using Microsoft.Extensions.Configuration;
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

            // TODO: get settings here from configuration
            var serviceBusReceiver = IdeaFactory.GetServiceBusReceiver(
                config["ServiceBus:Subscription"],
                config["ServiceBus:ConnectionString"],
                config["ServiceBus:TopicName"]);
            var wordPressClient = IdeaFactory.GetWordPressClient(config["WordPressUrl"]);
            var ideaRepository = IdeaFactory.GetIdeaRepository(config["IdeasApi"]);

            var mailmanSheetReader = new MailmanEnabledSheetReader(
                config["Notification:serviceAccountPrivateKey"],
                config["Notification:serviceAccountEmail"],
                config["Notification:spreadsheetId"]
                );

            var emailService = new EmailService();

            // Register listener
            serviceBusReceiver.ReceiveMessagesAsync(new IdeaLoggedListener(
                ideaRepository, 
                wordPressClient, 
                mailmanSheetReader, 
                emailService,
                config["Notification:MergeTemplate"]));

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

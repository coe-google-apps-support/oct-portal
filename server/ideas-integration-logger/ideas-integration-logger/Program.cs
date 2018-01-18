﻿using CoE.Ideas.Core;
using CoE.Ideas.Core.ServiceBus;
using CoE.Ideas.Core.WordPress;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Threading;

namespace CoE.Ideas.Integration.Logger
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
            services.AddIdeaListener<NewIdeaListener>(
                config["ServiceBus:ConnectionString"],
                config["ServiceBus:TopicName"],
                config["ServiceBus:Subscription"]);
            services.AddSingleton<IActiveDirectoryUserService, ActiveDirectoryUserService>(x =>
            {
                return new ActiveDirectoryUserService(
                    config["ActiveDirectory:Domain"],
                    config["ActiveDirectory:ServiceUserName"],
                    config["ActiveDirectory:ServicePassword"]);
            });
            services.AddIdeaServiceBusSender(
                config["ServiceBus:ConnectionString"],
                config["ServiceBus:TopicName"]);

            services.AddSingleton<IIdeaLogger, IIdeaLogger>(x =>
            {
                return new GoogleSheetIdeaLogger(
                    config["Logger:serviceAccountPrivateKey"],
                    config["Logger:serviceAccountEmail"],
                    config["Logger:spreadsheetId"],
                    config["IdeasApi"]);
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

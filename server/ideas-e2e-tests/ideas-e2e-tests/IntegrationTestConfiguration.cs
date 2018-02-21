using System;
using System.ServiceModel;
using AutoMapper;
using CoE.Ideas.Core;
using CoE.Ideas.Core.People;
using CoE.Ideas.Core.Security;
using CoE.Ideas.Core.ServiceBus;
using CoE.Ideas.Core.WordPress;
using CoE.Ideas.EndToEnd.Tests.IntegrationServices;
using CoE.Ideas.Remedy;
using CoE.Ideas.Remedy.RemedyServiceReference;
using CoE.Ideas.Remedy.SbListener;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace CoE.Ideas.EndToEnd.Tests
{
    internal class IntegrationTestConfiguration : BaseTestConfiguration
    {

        public IntegrationTestConfiguration(IConfigurationRoot config) : base(config)
        {
        }

        public new IntegrationTestConfiguration ConfigureBasicServices()
        {
            base.ConfigureBasicServices();

            // PeopleService is used by multiple services
            Services.AddPeopleService(Configuration["PeopleServiceUrl"]);


            return this;
        }

        public IntegrationTestConfiguration ConfigureIdeaServices()
        {
            Services.AddIdeaConfiguration(
                dbConnectionString: Configuration.GetConnectionString("IdeaDatabase"), 
                wordPressUrl: Configuration["Ideas:WordPressUrl"],
                jwtSecretKey: Configuration["Authorization:JwtSecretKey"]);

            Services.AddInitiativeMessaging(Configuration.GetConnectionString("IdeaServiceBus"),
                Configuration["Ideas:ServiceBusTopic"]);

            Services.AddAutoMapper();

            base.ConfigureIdeasController();

            return this;
        }

        public IntegrationTestConfiguration ConfigureRemedyServices()
        {

            ConfigureOnNewInitiativeRemedyServices();
            ConfigureOnWorkOrderUpdatedRemedyServices();
            ConfigureOnRemedyItemChangedServices();

            return this;
        }


        private void ConfigureOnNewInitiativeRemedyServices()
        {
            Services.AddSingleton<CoE.Ideas.Remedy.RemedyServiceReference.New_Port_0PortType>(x =>
            {
                return new New_Port_0PortTypeClient(
                    new BasicHttpBinding(BasicHttpSecurityMode.None)
                    {
                        MaxReceivedMessageSize = 16777216L // 16 MB, default it 65kb
                    },
                    new EndpointAddress(Configuration["Remedy:ApiUrl"]));
            });



            Services.Configure<RemedyServiceOptions>(Configuration.GetSection("Remedy"));
            Services.AddSingleton<IntegrationRemedyService>();
            Services.AddSingleton<IRemedyService>(x => x.GetRequiredService<IntegrationRemedyService>());


            // configure the listener that listens for new initiatives and create work order in remedy
            Services.AddSingleton<IntegrationRemedyListenerNewIdeaListener>(x =>
            {
                var subscriptionClient = new SubscriptionClient(connectionString: Configuration.GetConnectionString("IdeaServiceBus"),
                    topicPath: Configuration["Ideas:ServiceBusTopic"],
                    subscriptionName: Configuration["Ideas:RemedyServiceBusSubscription"]);

                var messageReceiver = new InitiativeMessageReceiver(x.GetRequiredService<IIdeaRepository>(),
                    x.GetRequiredService<IWordPressClient>(),
                    subscriptionClient, x.GetRequiredService<IJwtTokenizer>());

                return new IntegrationRemedyListenerNewIdeaListener(messageReceiver,
                    x.GetRequiredService<IInitiativeMessageSender>(),
                    x.GetRequiredService<IRemedyService>(),
                    x.GetRequiredService<Serilog.ILogger>());
            });
            Services.AddSingleton<NewIdeaListener, IntegrationRemedyListenerNewIdeaListener>(x => x.GetRequiredService<IntegrationRemedyListenerNewIdeaListener>());
            Services.AddSingleton<RemedyItemUpdatedIdeaListener>();
        }

        private void ConfigureOnWorkOrderUpdatedRemedyServices()
        {
            Services.AddSingleton<Remedy.Watcher.IRemedyService, Remedy.Watcher.RemedyService>();
            Services.Configure<Remedy.Watcher.RemedyCheckerOptions>(Configuration.GetSection("Remedy"));
            Services.AddSingleton<Remedy.Watcher.RemedyServiceReference.New_Port_0PortType>(x =>
            {
                return new Remedy.Watcher.RemedyServiceReference.New_Port_0PortTypeClient(
                    new BasicHttpBinding(BasicHttpSecurityMode.None)
                    {
                        MaxReceivedMessageSize = 16777216L // 16 MB, default it 65kb
                    },
                    new EndpointAddress(Configuration["Remedy:ApiSearchUrl"]));
            });

            Services.AddSingleton<IntegrationRemedyChecker>(x =>
            {
                return new IntegrationRemedyChecker(x.GetRequiredService<Remedy.Watcher.IRemedyService>(),
                    x.GetRequiredService<IInitiativeMessageSender>(),
                    x.GetRequiredService<IPeopleService>(),
                    x.GetRequiredService<Serilog.ILogger>(),
                    x.GetRequiredService<IOptions<Remedy.Watcher.RemedyCheckerOptions>>());

            });
            Services.AddSingleton<Remedy.Watcher.IRemedyChecker>(x => x.GetRequiredService<IntegrationRemedyChecker>());
        }


        private void ConfigureOnRemedyItemChangedServices()
        {
            // IntegrationRemedyItemUpdatedIdeaListener
            Services.AddSingleton<IntegrationRemedyItemUpdatedIdeaListener>(x =>
            {
                var subscriptionClient = new SubscriptionClient(connectionString: Configuration.GetConnectionString("IdeaServiceBus"),
                    topicPath: Configuration["Ideas:ServiceBusTopic"],
                    subscriptionName: Configuration["Ideas:RemedyCheckerServiceBusSubscription"]);

                var messageReceiver = new InitiativeMessageReceiver(x.GetRequiredService<IIdeaRepository>(),
                    x.GetRequiredService<IWordPressClient>(),
                    subscriptionClient, x.GetRequiredService<IJwtTokenizer>());

            return new IntegrationRemedyItemUpdatedIdeaListener(
                    x.GetRequiredService<IUpdatableIdeaRepository>(),
                    messageReceiver, 
                    x.GetRequiredService<Serilog.ILogger>());

            });
            Services.AddSingleton<RemedyItemUpdatedIdeaListener>(x => x.GetRequiredService<IntegrationRemedyItemUpdatedIdeaListener>());

        }
    }
}
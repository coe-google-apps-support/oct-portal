using System;
using System.ServiceModel;
using AutoMapper;
using CoE.Ideas.Core;
using CoE.Ideas.Remedy;
using CoE.Ideas.Remedy.RemedyServiceReference;
using CoE.Ideas.Remedy.SbListener;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
            return this;
        }

        public IntegrationTestConfiguration ConfigureIdeaServices()
        {
            Services.AddIdeaConfiguration(
                dbConnectionString: Configuration.GetConnectionString("IdeaDatabase"), 
                wordPressUrl: Configuration["Ideas:WordPressUrl"],
                jwtSecretKey: Configuration["Authorization:JwtSecretKey"]);

            Services.AddInitiativeMessaging(Configuration.GetConnectionString("IdeaServiceBus"),
                Configuration["Ideas:ServiceBusTopic"],
                Configuration["Ideas:ServiceBusSubscription"]);

            Services.AddAutoMapper();

            base.ConfigureIdeasController();

            return this;
        }

        public IntegrationTestConfiguration ConfigureRemedyServices()
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
            Services.AddSingleton<IntegrationRemedyListenerNewIdeaListener>();
            Services.AddSingleton<NewIdeaListener, IntegrationRemedyListenerNewIdeaListener>(x => x.GetRequiredService<IntegrationRemedyListenerNewIdeaListener>());
            Services.AddSingleton<RemedyItemUpdatedIdeaListener>();

            // Remedy Checker
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

            Services.AddSingleton<Remedy.Watcher.IRemedyChecker, Remedy.Watcher.RemedyChecker>();




            return this;
        }
    }
}
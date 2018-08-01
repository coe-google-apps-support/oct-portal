using CoE.Ideas.Core;
using CoE.Ideas.Core.ServiceBus;
using CoE.Ideas.Remedy.RemedyServiceReference;
using CoE.Ideas.Remedy.Watcher;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Text;

namespace CoE.Ideas.Remedy.Tests
{
    public class IntegrationTestConfiguration
    {
        public IntegrationTestConfiguration(IConfigurationRoot configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException("configuration");
            _services = new ServiceCollection();
        }

        public IntegrationTestConfiguration(IConfigurationRoot configuration,
            IServiceCollection services)
        {
            _configuration = configuration ?? throw new ArgumentNullException("configuration");
            _services = services ?? throw new ArgumentNullException("services");
        }

        private readonly IConfigurationRoot _configuration;
        private readonly IServiceCollection _services;

        public ServiceProvider ServiceProvider { get; private set; }

        public ServiceProvider BuildServiceProvider()
        {
            ServiceProvider = _services.BuildServiceProvider();
            return ServiceProvider;
        }

        public IntegrationTestConfiguration ConfigureBasicServices()
        {
            _services.AddOptions();

            // Add logging
            _services.AddSingleton(new LoggerFactory()
                .AddConsole(
                    Enum.Parse<LogLevel>(_configuration["Logging:Debug:LogLevel:Default"]),
                    bool.Parse(_configuration["Logging:IncludeScopes"]))
                .AddDebug(
                    Enum.Parse<LogLevel>(_configuration["Logging:Console:LogLevel:Default"])));
            _services.AddLogging();

            // configure application specific logging
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .Enrich.WithProperty("Application", "Initiatives")
                .Enrich.WithProperty("Module", "Logging")
                //.ReadFrom.Configuration(_configuration)
                .WriteTo.Console()
                .CreateLogger();

            _services.AddSingleton(x => Log.Logger);

            return this;
        }

        public IntegrationTestConfiguration ConfigureIdeaServices()
        {
            //_services.AddRemoteIdeaConfiguration(_configuration["IdeasApi"],
            //    _configuration["WordPressUrl"]);

            _services.AddSingleton<IInitiativeMessageSender, MockInitiativeMessageSender>();

            //_services.AddIdeaListener<NewIdeaListener>(
            //    _configuration["ServiceBus:ConnectionString"],
            //    _configuration["ServiceBus:TopicName"],
            //    _configuration["ServiceBus:Subscription"]);
            //_services.AddSingleton<IActiveDirectoryUserService, ActiveDirectoryUserService>(x =>
            //{
            //    return new ActiveDirectoryUserService(
            //        _configuration["ActiveDirectory:Domain"],
            //        _configuration["ActiveDirectory:ServiceUserName"],
            //        _configuration["ActiveDirectory:ServicePassword"]);
            //});

            return this;
        }

        public IntegrationTestConfiguration ConfigureRemedyServices()
        {
            _services.AddSingleton< CoE.Ideas.Remedy.RemedyServiceReference.New_Port_0PortType>(x =>
            {
                return new New_Port_0PortTypeClient(
                    new BasicHttpBinding(BasicHttpSecurityMode.None)
                    {
                        MaxReceivedMessageSize = 16777216L // 16 MB, default it 65kb
                    },
                    new EndpointAddress(_configuration["Remedy:ApiUrl"]));
            });



            _services.Configure<RemedyServiceOptions>(_configuration.GetSection("Remedy"));
            _services.AddSingleton<IRemedyService, RemedyService>();

            // Remedy Checker
            _services.AddSingleton<Watcher.IRemedyService, Watcher.RemedyService>();
            _services.Configure<Watcher.RemedyCheckerOptions>(_configuration.GetSection("Remedy"));
            _services.AddSingleton<Watcher.RemedyServiceReference.New_Port_0PortType>(x =>
            {
                return new Watcher.RemedyServiceReference.New_Port_0PortTypeClient(
                    new BasicHttpBinding(BasicHttpSecurityMode.None)
                    {
                        MaxReceivedMessageSize = 16777216L // 16 MB, default it 65kb
                    },
                    new EndpointAddress(_configuration["Remedy:ApiSearchUrl"]));
            }); 

            _services.AddSingleton<IRemedyChecker, RemedyChecker>();

            return this;
        }
    }
}

using CoE.Ideas.Core;
using CoE.Ideas.Core.ServiceBus;
using CoE.Ideas.Core.Tests;
using CoE.Ideas.Core.WordPress;
using CoE.Ideas.Remedy;
using CoE.Ideas.Server.Controllers;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoE.Ideas.EndToEnd.Tests
{
    public class TestConfiguration
    {
        public TestConfiguration(IConfigurationRoot configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException("configuration");
            _services = new ServiceCollection();
        }

        public TestConfiguration(IConfigurationRoot configuration,
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

        public TestConfiguration ConfigureBasicServices()
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
            _services.AddSingleton<Serilog.ILogger>(x => new LoggerConfiguration()
                .Enrich.FromLogContext()
                .Enrich.WithProperty("Application", "Initiatives")
                .Enrich.WithProperty("Module", "Logging")
                .ReadFrom.Configuration(_configuration)
                .CreateLogger());


            return this;
        }

        public TestConfiguration ConfigureIdeaServices()
        {
            _services.AddScoped<IWordPressClient, MockWordPressClient>();
            _services.AddSingleton<IIdeaServiceBusSender, MockIdeaServiceBusSender>();
            _services.AddScoped<IIdeaRepository, MockIdeaRepository>();
            _services.AddScoped<IdeasController>();
            return this;
        }

        public TestConfiguration ConfigureServiceBus()
        {
            _services.AddSingleton<ITopicClient, MockTopicClient>();
            return this;
        }

        public TestConfiguration ConfigureRemedyServices()
        {
            _services.AddSingleton<IRemedyService, MockRemedyService>();
            //_services.AddSingleton<NewIdeaListener>();

            //_services.AddSingleton(x =>
            //{
            //    return new New_Port_0PortTypeClient(
            //        new BasicHttpBinding(BasicHttpSecurityMode.None),
            //        new EndpointAddress(_configuration["Remedy:ApiUrl"]));
            //});
            //_services.Configure<RemedyServiceOptions>(options =>
            //{
            //    options.CategorizationTier1 = _configuration["Remedy:CategorizationTier1"];
            //    options.CategorizationTier2 = _configuration["Remedy:CategorizationTier2"];
            //    options.LocationCompany = _configuration["Remedy:LocationCompany"];
            //    options.CustomerCompany = _configuration["Remedy:CustomerCompany"];
            //    options.ServicePassword = _configuration["Remedy:ServicePassword"];
            //    options.ServiceUserName = _configuration["Remedy:ServiceUserName"];
            //    options.TemplateId = _configuration["Remedy:TemplateId"];
            //    options.CustomerLoginId = _configuration["Remedy:CustomerLoginId"];
            //    options.CustomerFirstName = _configuration["Remedy:CustomerFirstName"];
            //    options.CustomerLastName = _configuration["Remedy:CustomerLastName"];
            //});
            //_services.AddSingleton<IRemedyService, RemedyService>();

            return this;
        }
    }
}

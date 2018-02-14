using CoE.Ideas.Core;
using CoE.Ideas.Remedy.RemedyServiceReference;
using CoE.Ideas.Remedy.Watcher;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Text;

namespace CoE.Ideas.Remedy.Tests
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
            // basic stuff - there's probably a better way to register these
            _services.AddSingleton(
                typeof(Microsoft.Extensions.Options.IOptions<>),
                typeof(Microsoft.Extensions.Options.OptionsManager<>));
            _services.AddSingleton(
                typeof(Microsoft.Extensions.Options.IOptionsFactory<>),
                typeof(Microsoft.Extensions.Options.OptionsFactory<>));

            return this;
        }

        public TestConfiguration ConfigureIdeaServices()
        {
            _services.AddRemoteIdeaConfiguration(_configuration["IdeasApi"],
                _configuration["WordPressUrl"]);
            _services.AddIdeaListener<NewIdeaListener>(
                _configuration["ServiceBus:ConnectionString"],
                _configuration["ServiceBus:TopicName"],
                _configuration["ServiceBus:Subscription"]);
            _services.AddSingleton<IActiveDirectoryUserService, ActiveDirectoryUserService>(x =>
            {
                return new ActiveDirectoryUserService(
                    _configuration["ActiveDirectory:Domain"],
                    _configuration["ActiveDirectory:ServiceUserName"],
                    _configuration["ActiveDirectory:ServicePassword"]);
            });

            return this;
        }

        public TestConfiguration ConfigureRemedyServices()
        {
            _services.AddSingleton(x =>
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

          //  _services.AddSingleton<New_Port_0PortType>

            _services.AddSingleton<IRemedyChecker, RemedyChecker>();

            return this;
        }
    }
}

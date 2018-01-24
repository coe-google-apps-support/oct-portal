using CoE.Ideas.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
            //_services.AddRemoteIdeaConfiguration(_configuration["IdeasApi"],
            //    _configuration["WordPressUrl"]);
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

        public TestConfiguration ConfigureRemedyServices()
        {
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

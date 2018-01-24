using AutoMapper;
using CoE.Ideas.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoE.Ideas.Server.Tests
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
            _services.AddIdeaConfiguration(
                _configuration.GetConnectionString("IdeaDatabase"),
                _configuration["Ideas:WordPressUrl"],
                _configuration.GetConnectionString("IdeaServiceBus"),
                _configuration["Ideas:ServiceBusTopic"]);


            _services.AddIdeaAuthSecurity(
                _configuration["Authorization:JwtSecretKey"],
                _configuration["Authorization:CoeAuthKey"],
                _configuration["Authorization:CoeAuthIV"],
                _configuration["Ideas:WordPressUrl"]);

            _services.AddMvc();

            _services.AddAutoMapper();
            return this;
        }


    }
}

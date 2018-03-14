using AutoMapper;
using CoE.Ideas.Core.Data;
using CoE.Ideas.Core.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace CoE.Ideas.Core.Tests
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

        internal TestConfiguration ConfigureIdeaServicesInMemory()
        {
            _services.AddDbContext<InitiativeContext>(x => x.UseInMemoryDatabase("ideas"));
            _services.AddScoped<IInitiativeRepository, LocalInitiativeRepository>();

            //_services.AddSingleton<IWordPressClient, MockWordPressClient>();
            //_services.AddSingleton<ITopicSender<IdeaMessage>, NullTopicSender<IdeaMessage>>();

            //_services.AddSingleton<IIdeaServiceBusSender, IdeaServiceBusSender>();

            _services.AddAutoMapper();
            return this;
        }
    }
}

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using CoE.Ideas.Core.Data;
using Microsoft.EntityFrameworkCore;
using CoE.Ideas.Core.Services;
using AutoMapper;
using Serilog;
using CoE.Ideas.Core.Events;
using MediatR;
using CoE.Ideas.Shared.Security;
using DateTimeExtensions.WorkingDays;

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
            _services.AddOptions();
            _services.AddMemoryCache();
            _services.AddMediatR();

            // configure application specific logging
            _services.AddSingleton<Serilog.ILogger>(x => new LoggerConfiguration()
                .Enrich.FromLogContext()
                .Enrich.WithProperty("Application", "ideas-core-xtests")
                .Enrich.WithProperty("Module", "Server")
                .ReadFrom.Configuration(_configuration)
                .WriteTo.Console()
                .CreateLogger());

            _services.AddSingleton<ICurrentUserAccessor, MockCurrentUserAccessor>();

            return this;
        }

        internal TestConfiguration ConfigureIdeaServicesInMemory()
        {
            _services.AddDbContext<InitiativeContext>(x => x.UseInMemoryDatabase("ideas"));
            _services.AddScoped<IInitiativeRepository, LocalInitiativeRepository>();

            _services.AddSingleton<DomainEvents>();

            //_services.AddSingleton<IWordPressClient, MockWordPressClient>();
            //_services.AddSingleton<ITopicSender<IdeaMessage>, NullTopicSender<IdeaMessage>>();

            //_services.AddSingleton<IIdeaServiceBusSender, IdeaServiceBusSender>();

            //_services.AddAutoMapper();
            return this;
        }

        internal TestConfiguration ConfigureBusinessCalendarService(string payrollCalenderServiceUrl = null)
        {
            _services.AddMemoryCache();
            _services.AddSingleton<BusinessCalendarService>();
            _services.AddSingleton<IBusinessCalendarService>(x => x.GetRequiredService<BusinessCalendarService>());
            _services.AddSingleton<IHolidayStrategy>(x => x.GetRequiredService<BusinessCalendarService>());
            return this;
        }
    }
}

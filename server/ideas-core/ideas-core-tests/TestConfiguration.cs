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
            _services.AddMediatR();

            // configure application specific logging
            _services.AddSingleton<Serilog.ILogger>(x => new LoggerConfiguration()
                .Enrich.FromLogContext()
                .Enrich.WithProperty("Application", "ideas-core-xtests")
                .Enrich.WithProperty("Module", "Server")
                .ReadFrom.Configuration(_configuration)
                .CreateLogger());

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
            string calendarServiceUrl = string.IsNullOrWhiteSpace(payrollCalenderServiceUrl)
                ? "http://webapps1.edmonton.ca/CoE.PayrollCalendar.WebApi/api/PayrollCalendar" : payrollCalenderServiceUrl;
            _services.Configure<BusinessCalendarServiceOptions>(x => x.PayrollCalenderServiceUrl = calendarServiceUrl);
            _services.AddSingleton<IBusinessCalendarService, BusinessCalendarService>();
            return this;
        }
    }
}

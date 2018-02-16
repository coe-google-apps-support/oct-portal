using CoE.Ideas.Core;
using CoE.Ideas.Core.ServiceBus;
using CoE.Ideas.Core.Tests;
using CoE.Ideas.Core.WordPress;
using CoE.Ideas.Integration.Notification;
using CoE.Ideas.Remedy;
using CoE.Ideas.Remedy.RemedyServiceReference;
using CoE.Ideas.Remedy.SbListener;
using CoE.Ideas.Server.Controllers;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.ServiceModel;
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

        public TestConfiguration ConfigureIdeaServices()
        {
            _services.AddScoped<IWordPressClient, MockWordPressClient>();
            _services.AddScoped<IIdeaRepository, MockIdeaRepository>();
            _services.AddScoped<IUpdatableIdeaRepository, MockIdeaRepository>();
            _services.AddScoped<IdeasController>(x =>
            {
                return new IdeasController(x.GetRequiredService<IUpdatableIdeaRepository>(),
                    x.GetRequiredService<IWordPressClient>(),
                    x.GetRequiredService<IInitiativeMessageSender>(),
                    x.GetRequiredService<Serilog.ILogger>())
                {
                    ControllerContext = new Microsoft.AspNetCore.Mvc.ControllerContext() {
                        HttpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext
                        {
                            User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                            {
                                new Claim(ClaimTypes.Name, "Snow White"),
                                new Claim(ClaimTypes.Email, "snow.white@edmonton.ca")
                            }, "someAuthTypeName"))
                        }
                    }
                };
            });
            return this;
        }

        public TestConfiguration ConfigureIdeaMessaging()
        {

            _services.AddSingleton<MockInitiativeMessageReceiver>();
            _services.AddSingleton<IInitiativeMessageReceiver>(x =>
            {
                return x.GetRequiredService<MockInitiativeMessageReceiver>();
            });
            _services.AddSingleton<IInitiativeMessageSender, MockInitiativeMessageSender>();

            return this;
        }

        public TestConfiguration ConfigureRemedyServices()
        {
            _services.AddSingleton<MockRemedyService>();
            _services.AddSingleton<IRemedyService>(x => x.GetRequiredService<MockRemedyService>());

            // configure the listener that listens for new initiatives and create work order in remedy
            _services.AddSingleton<NewIdeaListener>();

            _services.AddSingleton<RemedyItemUpdatedIdeaListener>();

            return this;
        }

        public TestConfiguration ConfigureNotificationServices()
        {
            // logger first
            _services.AddSingleton<MockIdeaLogger>();
            _services.AddSingleton<Integration.Logger.IIdeaLogger>(x => x.GetRequiredService<MockIdeaLogger>());
            _services.AddSingleton<Integration.Logger.NewIdeaListener>();

            // now notifications
            _services.AddSingleton<MockMailmanEnabledSheetReader>();
            _services.AddSingleton<IMailmanEnabledSheetReader>(x => x.GetRequiredService<MockMailmanEnabledSheetReader>());
            _services.AddSingleton<MockEmailService>();
            _services.AddSingleton<IEmailService>(x => x.GetRequiredService<MockEmailService>());
            _services.AddSingleton(x =>
            {
                return new IdeaLoggedListener(
                    x.GetRequiredService<IMailmanEnabledSheetReader>(),
                    x.GetRequiredService<IEmailService>(),
                    x.GetRequiredService<IInitiativeMessageReceiver>(),
                    x.GetRequiredService<Serilog.ILogger>(),
                    "TestMergeTemplate");
            });


            return this;
        }
    }
}

using CoE.Ideas.Core;
using CoE.Ideas.Core.Data;
using CoE.Ideas.Core.ServiceBus;
using CoE.Ideas.Core.Services;
using CoE.Ideas.Core.Tests;
using CoE.Ideas.Server.Controllers;
using CoE.Ideas.Shared.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Serilog;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

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
            _services.AddOptions();

            // Add logging

            //_services.AddSingleton(new LoggerFactory()
            //    .AddConsole(
            //        Enum.Parse<LogLevel>(_configuration["Logging:Debug:LogLevel:Default"]),
            //        bool.Parse(_configuration["Logging:IncludeScopes"]))
            //    .AddDebug(
            //        Enum.Parse<LogLevel>(_configuration["Logging:Console:LogLevel:Default"])));
            //_services.AddLogging();

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
            //_services.AddScoped<IWordPressClient, MockWordPressClient>();
            _services.AddScoped<IInitiativeRepository, MockIdeaRepository>();

            //_services.AddMvc();

            //_services.AddAutoMapper();
            return this;
        }

        public TestConfiguration ConfigureIdeaMessaging()
        {
            _services.AddInitiativeMessaging();
            return this;
        }

        public ClaimsPrincipal CurrentUser
        {
            get
            {
                return SnowWhite;
            }
        }

        private ClaimsPrincipal showWhite;
        protected ClaimsPrincipal SnowWhite
        {
            get
            {
                if (showWhite == null)
                {
                    showWhite = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.Name, "Snow White"),
                        new Claim(ClaimTypes.Email, "snow.white@edmonton.ca")
                    }, "someAuthTypeName"));
                }
                return showWhite;
            }
        }

        private ClaimsPrincipal sleepingBeauty;
        protected ClaimsPrincipal SleepingBeauty
        {
            get
            {
                if (sleepingBeauty == null)
                {
                    sleepingBeauty = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.Name, "Sleeping Beauty"),
                        new Claim(ClaimTypes.Email, "sleepingbeauty@edmonton.ca")
                    }, "someAuthTypeName"));
                }
                return sleepingBeauty;
            }
        }

        public TestConfiguration ConfigureControllers()
        {
            _services.AddScoped<IdeasController>(x =>
            {
                return new IdeasController(x.GetRequiredService<IInitiativeRepository>(),
                    x.GetRequiredService<IPersonRepository>(),
                    x.GetRequiredService<IStringTemplateService>(),
                    x.GetRequiredService<IInitiativeService>(),
                    x.GetRequiredService<Serilog.ILogger>())
                {
                    ControllerContext = new Microsoft.AspNetCore.Mvc.ControllerContext()
                    {
                        HttpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext
                        {
                            User = CurrentUser
                        }
                    }
                };
            });

            return this;
        }

        public async Task<TestConfiguration> SetupMockData(ServiceProvider serviceProvider)
        {
            var repository = serviceProvider.GetRequiredService<IInitiativeRepository>();
            var personRepository = serviceProvider.GetRequiredService<IPersonRepository>();
            int snowWhiteId = await personRepository.GetPersonIdByEmailAsync(SnowWhite.GetEmail());
            int sleepingBeautyId = await personRepository.GetPersonIdByEmailAsync(SleepingBeauty.GetEmail());

            await repository.AddInitiativeAsync(Initiative.Create(title: "Test Idea 1", description: "Test Idea 1 Description ", ownerPersonId: snowWhiteId));
            await repository.AddInitiativeAsync(Initiative.Create(title: "Test Idea 2", description: "Test Idea 2 Description ", ownerPersonId: snowWhiteId));
            await repository.AddInitiativeAsync(Initiative.Create(title: "Test Idea 3", description: "Test Idea 3 Description ", ownerPersonId: sleepingBeautyId));

            return this;
        }
    }
}

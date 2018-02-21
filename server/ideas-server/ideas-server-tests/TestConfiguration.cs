using AutoMapper;
using CoE.Ideas.Core;
using CoE.Ideas.Core.ServiceBus;
using CoE.Ideas.Core.Tests;
using CoE.Ideas.Core.WordPress;
using CoE.Ideas.Server.Controllers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
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
            _services.AddScoped<IWordPressClient, MockWordPressClient>();
            _services.AddScoped<IIdeaRepository, MockIdeaRepository>();
            _services.AddScoped<IUpdatableIdeaRepository, MockIdeaRepository>();


            //_services.AddIdeaAuthSecurity(
            //    _configuration["Authorization:JwtSecretKey"],
            //    _configuration["Authorization:CoeAuthKey"],
            //    _configuration["Authorization:CoeAuthIV"],
            //    _configuration["Ideas:WordPressUrl"]);

            //_services.AddMvc();

            //_services.AddAutoMapper();
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



        private ClaimsPrincipal currentUser;
        public ClaimsPrincipal CurrentUser
        {
            get
            {
                if (currentUser == null)
                {
                    currentUser = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.Name, "Snow White"),
                        new Claim(ClaimTypes.Email, "snow.white@edmonton.ca")
                    }, "someAuthTypeName"));
                }
                return currentUser;
            }
        }

        public TestConfiguration ConfigureControllers()
        {
            _services.AddScoped<IdeasController>(x =>
            {
                return new IdeasController(x.GetRequiredService<IUpdatableIdeaRepository>(),
                    x.GetRequiredService<IWordPressClient>(),
                    x.GetRequiredService<IInitiativeMessageSender>(),
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
            var repository = serviceProvider.GetRequiredService<IUpdatableIdeaRepository>();

            var snowWhite = new Stakeholder() { Email = CurrentUser.GetEmail(), UserName = CurrentUser.GetDisplayName(), Type = "owner" };
            var sleepingBeauty = new Stakeholder() { Email = "sleepingbeauty@edmonton.ca", UserName = "Sleeping Beauty", Type = "owner" };

            await repository.AddIdeaAsync(new Idea() { Title = "Test Idea 1", Description = "Test Idea 1 Description " }, snowWhite);
            await repository.AddIdeaAsync(new Idea() { Title = "Test Idea 2", Description = "Test Idea 2 Description " }, snowWhite);
            await repository.AddIdeaAsync(new Idea() { Title = "Test Idea 3", Description = "Test Idea 3 Description " }, sleepingBeauty);

            return this;
        }
    }
}

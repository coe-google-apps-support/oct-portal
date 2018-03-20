using CoE.Ideas.Core.ServiceBus;
using CoE.Ideas.Core.Services;
using CoE.Ideas.Server.Controllers;
using CoE.Ideas.Shared.WordPress;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;
using System.Security.Claims;

namespace CoE.Ideas.EndToEnd.Tests
{
    public class BaseTestConfiguration
    {
        public BaseTestConfiguration(IConfigurationRoot configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException("configuration");
            _services = new ServiceCollection();
        }

        public BaseTestConfiguration(IConfigurationRoot configuration,
            IServiceCollection services) 
        {
            _configuration = configuration ?? throw new ArgumentNullException("configuration");
            _services = services ?? throw new ArgumentNullException("services");
        }

        private readonly IConfigurationRoot _configuration;
        private readonly IServiceCollection _services;

        protected IConfigurationRoot Configuration {  get { return _configuration; } }
        protected IServiceCollection Services {  get { return _services; } }

        public virtual BaseTestConfiguration ConfigureBasicServices()
        {
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

        protected virtual BaseTestConfiguration ConfigureIdeasController()
        {
            Services.AddScoped<IdeasController>(x =>
            {
                return new IdeasController(x.GetRequiredService<IInitiativeRepository>(),
                    x.GetRequiredService<IPersonRepository>(),
                    x.GetRequiredService<IInitiativeMessageSender>(),
                    x.GetRequiredService<Serilog.ILogger>())
                {
                    ControllerContext = new Microsoft.AspNetCore.Mvc.ControllerContext()
                    {
                        HttpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext
                        {
                            User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                            {
                                new Claim(ClaimTypes.Name, "Snow White"),
                                new Claim(ClaimTypes.Email, "snow.white@edmonton.ca"),
                                new Claim("data", "\"user\": { \"id\": \"6\" }") // <-- WordPress specific
                            }, "someAuthTypeName"))
                        }
                    }
                };
            });
            return this;
        }


        public ServiceProvider BuildServiceProvider()
        {
            return Services.BuildServiceProvider();
        }

    }
}

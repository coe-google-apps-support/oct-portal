using EnsureThat;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace CoE.Issues.Core.Tests
{
    public class TestConfiguration
    {
        public TestConfiguration(IConfigurationRoot configuration)
        {
            EnsureArg.IsNotNull(configuration);
            _configuration = configuration;
            _services = new ServiceCollection();
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

            // configure application specific logging
            _services.AddSingleton<Serilog.ILogger>(x => new LoggerConfiguration()
                .Enrich.FromLogContext()
                .Enrich.WithProperty("Application", "issues-core-tests")
                .Enrich.WithProperty("Module", "Server")
                .ReadFrom.Configuration(_configuration)
                .WriteTo.Console()
                .CreateLogger());

            return this;
        }

        internal TestConfiguration AddServicebus()
        {
            // Configuration["WordPress:Url"]
            _services.AddIssueMessaging(_configuration["ServiceBus:ConnectionString"], _configuration["ServiceBus:TopicName"], _configuration["ServiceBus:Subscription"]);
            return this;
        }
    }
}

using CoE.Ideas.Core;
using CoE.Ideas.Remedy;
using CoE.Ideas.Server.Controllers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Threading.Tasks;

namespace CoE.Ideas.EndToEnd.Tests
{
    [TestClass]
    public class HappyPathTests
    {
        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            context.WriteLine($"Environment is { Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") }");
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile($"appsettings.Development.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            serviceProvider = new TestConfiguration(config)
                .ConfigureBasicServices()
                .ConfigureServiceBus()
                .ConfigureIdeaServices()
                .ConfigureRemedyServices()
                .BuildServiceProvider();
        }
        private static ServiceProvider serviceProvider;



        [TestMethod]
        [TestCategory("End to End")]
        public async Task CreateInitiative()
        {
            var ideasController = serviceProvider.GetRequiredService<IdeasController>();
            await ideasController.PostIdea(new Idea()
            {
                Title = "Happy Path Test - Create Initiative",
                Description = "Happy Path Test - Create Initiative"
            });

            var remedyService = serviceProvider.GetRequiredService<IRemedyService>() as MockRemedyService;
           // Assert.IsTrue(remedyService.Items.Count > 0, "Item not created in Remedy");

        }
    }
}

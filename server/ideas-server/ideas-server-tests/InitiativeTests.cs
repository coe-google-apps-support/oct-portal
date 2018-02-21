using CoE.Ideas.Server.Controllers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CoE.Ideas.Server.Tests
{
    [TestClass]
    public class InitiativeTests
    {
        [ClassInitialize]
        public static async Task ClassInit(TestContext context)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            var testConfig = new TestConfiguration(config)
                .ConfigureBasicServices()
                .ConfigureIdeaServices()
                .ConfigureIdeaMessaging()
                .ConfigureControllers();

            serviceProvider = testConfig
                .BuildServiceProvider();

            await testConfig.SetupMockData(serviceProvider);

        }
        private static ServiceProvider serviceProvider;

        [TestMethod]
        public async Task TestReadInitiatives()
        {
            var ideasController = serviceProvider.GetRequiredService<IdeasController>();
            var allIdeas = await ideasController.GetIdeas();

            Assert.IsTrue(allIdeas != null && allIdeas.Count() == 3, "Expected to read at 3 initiatives (SetupMockData sets up 3 initiatives)");

            var myIdeas = await ideasController.GetIdeas(Models.ViewOptions.Mine);
            Assert.IsTrue(myIdeas != null && myIdeas.Count() == 3, "Exoected to get 2 initiatives when reading \"My Initiatives\" (SetupMockData sets up 2 initiatives as current user)");


        }
    }
}

using CoE.Ideas.Server.Controllers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
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
            var result = await ideasController.GetInitiatives();

            Assert.IsNotNull(result, "ideasController.GetInitiatives() returned null");
            var objectResult = result as Microsoft.AspNetCore.Mvc.ObjectResult;

            Assert.IsNotNull(objectResult, "ideasController.GetInitiatives() did not return an ObjectResult");
            var allIdeas = objectResult.Value as IEnumerable<Models.InitiativeInfo>;

            Assert.IsNotNull(allIdeas, "ideasController.GetInitiatives() did not return an object of type IEnumerable<Models.InitiativeInfo>");

            Assert.IsTrue(allIdeas != null && allIdeas.Count() == 3, "Expected to read at 3 initiatives (SetupMockData sets up 3 initiatives)");

            var result2 = await ideasController.GetInitiatives(Models.ViewOptions.Mine);
            var objectResult2 = result as Microsoft.AspNetCore.Mvc.ObjectResult;

            Assert.IsNotNull(objectResult2, "ideasController.GetInitiatives() did not return an ObjectResult");
            var myIdeas = objectResult2.Value as IEnumerable<Models.InitiativeInfo>;
            Assert.IsNotNull(myIdeas, "ideasController.GetInitiatives() did not return an object of type IEnumerable<Models.InitiativeInfo>");

            Assert.IsTrue(myIdeas != null && myIdeas.Count() == 2, "Expected to get 2 initiatives when reading \"My Initiatives\" (SetupMockData sets up 2 initiatives as current user)");


        }
    }
}

using CoE.Ideas.Core;
using CoE.Ideas.Core.WordPress;
using CoE.Ideas.Remedy;
using COE_WOI_WorkOrderInterface_WS;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.ServiceModel;
using System.Threading.Tasks;

namespace CoE.Ideas.Remedy.Tests
{
    [TestClass]
    public class IntegrationTests
    {
        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            serviceProvider = new TestConfiguration(config)
                .ConfigureBasicServices()
                .ConfigureRemedyServices()
                .BuildServiceProvider();
        }

        private static ServiceProvider serviceProvider;

        [TestMethod]
        [TestCategory("Integration")]
        public async Task TestCreateRemedyWorkOrder()
        {
            // mock Idea
            var newIdea = new Idea()
            {
                Title = "Test Idea 1",
                Description = "Test Idea 1 Contents"
            };

            // mock user 
            var newUser = new WordPressUser()
            {
                FirstName = "Jane",
                LastName = "Doe"
            };

            var remedyService = serviceProvider.GetRequiredService<IRemedyService>();
            await remedyService.PostNewIdeaAsync(newIdea, newUser, "COE\\fakeuser");
        }
    }
}

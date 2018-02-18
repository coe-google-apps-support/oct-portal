using CoE.Ideas.Core;
using CoE.Ideas.Core.WordPress;
using CoE.Ideas.Remedy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;
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
            context.WriteLine($"Environment is { Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") }");
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile($"appsettings.Development.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            serviceProvider = new IntegrationTestConfiguration(config)
                .ConfigureBasicServices()
                .ConfigureIdeaServices()
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
                Title = "Test Idea 2",
                Description = "Test Idea 2 Contents"
            };

            // mock user 
            var newUser = new WordPressUser()
            {
                FirstName = "Jane",
                LastName = "Doe"
            };

            var remedyService = serviceProvider.GetRequiredService<IRemedyService>();
            await remedyService.PostNewIdeaAsync(newIdea, "COE\\fakeuser");
        }

        [TestMethod]
        [TestCategory("Integration")]
        public async Task TestReadRemedyWorkOrders()
        {
            var remedyChecker = serviceProvider.GetRequiredService<Watcher.IRemedyChecker>();

            var items = await remedyChecker.PollAsync(DateTime.MinValue);

            Assert.IsTrue(items.ProcessErrors.Count == 0, $"{ items.ProcessErrors.Count } Errors encountered while polling remedy. First error was: { items.ProcessErrors.FirstOrDefault()?.ErrorMessage }");
            Assert.IsTrue(items.RecordsProcesed.Count > 0, "Expected at least one record processed");


        }
    }
}

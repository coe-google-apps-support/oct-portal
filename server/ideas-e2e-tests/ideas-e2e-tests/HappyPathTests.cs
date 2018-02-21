using CoE.Ideas.Core;
using CoE.Ideas.EndToEnd.Tests.Mocks;
using CoE.Ideas.Integration.Notification;
using CoE.Ideas.Remedy;
using CoE.Ideas.Remedy.SbListener;
using CoE.Ideas.Server.Controllers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;
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
                .ConfigureIdeaMessaging()
                .ConfigureIdeaServices()
                .ConfigureRemedyServices()
                .ConfigureNotificationServices()
                .BuildServiceProvider();

        }
        private static ServiceProvider serviceProvider;


        [TestInitialize]
        public void Initialize()
        {
            logger = serviceProvider.GetRequiredService<Serilog.ILogger>();
            ideasController = serviceProvider.GetRequiredService<IdeasController>();

            // service bus listeners - we need to get them to ensure they set up their message pumps
            // (in their respective constructors)
            var remedyNewIdeaService = serviceProvider.GetRequiredService<NewIdeaListener>();
            var remedySblistenerService = serviceProvider.GetRequiredService<RemedyItemUpdatedIdeaListener>();
            var loggerService = serviceProvider.GetRequiredService<Integration.Logger.NewIdeaListener>();
            var notificationService = serviceProvider.GetRequiredService<IdeaLoggedListener>();

        }

        private Serilog.ILogger logger;
        private IdeasController ideasController;


        [TestMethod]
        [TestCategory("End to End")]
        public async Task CreateInitiative()
        {
            // mock services
            var mockRemedyService = serviceProvider.GetRequiredService<MockRemedyService>();
            var mockLoggerService = serviceProvider.GetRequiredService<MockIdeaLogger>();
            var mockEmailService = serviceProvider.GetRequiredService<MockEmailService>();

            int initialIdeasLogged = mockLoggerService.InitiativesLogged.Count();
            int initialEmailsSent = mockEmailService.EmailsSent.Count();

            logger.Information("Starting Test CreateInitiative...");

            // simply posting a new idea should create the initiativa and sent the message on the service bus,
            // which trigger a bunch of other integration points which we'll verify after
            var newIdea = new Idea()
            {
                Title = "Happy Path Test - Create Initiative",
                Description = "Happy Path Test - Create Initiative"
            };
            await ideasController.PostIdea(newIdea);

            // here we verify the integration points. Note that each service is mocked so we don't actually
            // call the remote services, we just pretend to (mocking).

            // ensure we tried to create an item in "Remedy"
            Assert.IsTrue(mockRemedyService.WorkOrdersAdded.Count > 0, "Item not created in Remedy");

            // ensure our idea has the right WorkItemId
            Assert.IsTrue(!string.IsNullOrWhiteSpace(newIdea.WorkItemId), "Remedy did not assign a WorkOrderId");

            // ensure we logged one initiave
            Assert.IsTrue(mockLoggerService.InitiativesLogged.Count() == initialIdeasLogged + 1, "Expected 1 initiative logged");

            // ensure we "sent" one email
            Assert.IsTrue(mockEmailService.EmailsSent.Count() == initialEmailsSent + 1, "Expected to send one email");
        }
    }
}

using CoE.Ideas.Core;
using CoE.Ideas.EndToEnd.Tests.IntegrationServices;
using CoE.Ideas.Integration.Notification;
using CoE.Ideas.Remedy.SbListener;
using CoE.Ideas.Server.Controllers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CoE.Ideas.EndToEnd.Tests
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

        [TestInitialize]
        public void Initialize()
        {
            logger = serviceProvider.GetRequiredService<Serilog.ILogger>();
            ideasController = serviceProvider.GetRequiredService<IdeasController>();

            serviceProvider.GetRequiredService<Microsoft.AspNetCore.Http.IHttpContextAccessor>().HttpContext = ideasController.HttpContext;

            // service bus listeners - we need to get them to ensure they set up their message pumps
            // (in their respective constructors)
            //var remedySblistenerService = serviceProvider.GetRequiredService<RemedyItemUpdatedIdeaListener>();
            //var loggerService = serviceProvider.GetRequiredService<Integration.Logger.NewIdeaListener>();
            //var notificationService = serviceProvider.GetRequiredService<IdeaLoggedListener>();

        }

        private Serilog.ILogger logger;
        private IdeasController ideasController;


        [TestMethod]
        [TestCategory("Integration")]
        public async Task TestCreateRealInitiative()
        {
            DateTime startTime = DateTime.Now;

            // Create a new Initiative and use the Controller to post it
            var newIdeaTitle = "Test Idea " + DateTime.Now.ToString("MMM dd yyyy HH:mm:ss");
            var newInitiative = await CreateTestInitiativeAndPostToController(new Idea()
            {
                Title = newIdeaTitle,
                Description = "Test Idea description"
            });


            // Posting the new initiative should automatically create the Remedy Work Order
            string workOrderId = await EnsureRemedyWorkOrderCreated(newIdeaTitle);


            // Creating the Remedy Work Order should update the initiative with its Work Order Id
            await EnsureInitiativeUpdatedWithWorkOrderId(newIdeaTitle, workOrderId);

            // ensure the "RremedyChecker" verifies the Work Order was created, and send a message on the service bus
            // to notify Octava that the Work Order has been created
            var remedyChecker = serviceProvider.GetRequiredService<IntegrationRemedyChecker>();
            await remedyChecker.PollAsync(startTime.ToUniversalTime());
            Assert.AreEqual(1, remedyChecker.WorkOrdersProcessed.Count, $"Expected 1 new remedy item added since the test started at { startTime }");
        }


        private async Task<Idea> CreateTestInitiativeAndPostToController(Idea testInitiative)
        {

            var postIdeaResult = await ideasController.PostIdea(testInitiative) as Microsoft.AspNetCore.Mvc.ObjectResult;
            Assert.IsNotNull(postIdeaResult, "Could not create initiative");
            Assert.AreEqual(201, postIdeaResult.StatusCode, "IdeasController did not report success. Message returned was: {0}", postIdeaResult.Value);

            var createdAtResult = postIdeaResult as Microsoft.AspNetCore.Mvc.CreatedAtActionResult;
            Assert.IsNotNull(createdAtResult, $"Expected IdeasController to return an object of type CreatedAtActionResult, but got { postIdeaResult.GetType().FullName } ");

            var createdIdea = createdAtResult.Value as Idea;
            Assert.IsNotNull(createdIdea, $"Expected IdeasController to return an Idea object as its return value, but got { createdAtResult.Value?.GetType()?.FullName }");

            return createdIdea;
        }


        private async Task<string> EnsureRemedyWorkOrderCreated(string newInitiativeTitle)
        {
            // ensure the remedyNewIdeaService got the idea and posted a new Work Order in Remedy,
            // this should also send a message on the bus with the Remedy Work Order Id

            // we'll give 10 seconds to get the item from the service bus
            var timeout = new TimeSpan(0, 0, 10).TotalMilliseconds;
            Stopwatch watch = new Stopwatch();
            watch.Start();
            var remedyNewIdeaService = serviceProvider.GetRequiredService<IntegrationRemedyListenerNewIdeaListener>();
            try
            {
                while (true)
                {
                    if (watch.ElapsedMilliseconds >= timeout)
                    {
                        Assert.Fail("Timeout waiting for the Remedy NewIdeaListener to receive the new initiative");
                        break;
                    }
                    if (remedyNewIdeaService.NewInitiatives.Any(x => x.Title == newInitiativeTitle))
                        break;
                    logger.Information("Waiting for Remedy NewIdeaListener...");
                    Thread.Sleep(500); // wait half a second and try again
                }
                var remedyService = serviceProvider.GetRequiredService<IntegrationRemedyService>();
                var remedyAddedItem = (remedyService.WorkOrdersAdded.FirstOrDefault(x => x.Idea.Title == newInitiativeTitle));
                Assert.IsNotNull(remedyAddedItem, "New initiative did not create a Work Order in Remedy");
                Assert.IsTrue(!string.IsNullOrWhiteSpace(remedyAddedItem.WorkOrderId), "Remedy did not assign an Id to the work order created by a new initiative");
                return remedyAddedItem.WorkOrderId;
            }
            finally
            {
                await remedyNewIdeaService.CloseAsync();
            }
        }

        private async Task EnsureInitiativeUpdatedWithWorkOrderId(string newIdeaTitle, string workdOrderId)
        {

        }

        //[TestMethod]
        //[TestCategory("Integration")]
        //public async Task TestGetUpdatedStatus()
        //{
        //    // To do this test you need to manually update Remedy and change the below timestamp to just before the update
        //    DateTime updateDate = new DateTime(2018, 2, 21, 10, 6, 0);
        //    var remedyChecker = serviceProvider.GetRequiredService<IntegrationRemedyChecker>();
        //    await remedyChecker.PollAsync(updateDate.ToUniversalTime());
        //    Assert.AreEqual(1, remedyChecker.WorkOrdersProcessed.Count, "Expected 1 new remedy item added");
        //}
    }
}

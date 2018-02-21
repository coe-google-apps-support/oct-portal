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
        }

        private Serilog.ILogger logger;
        private IdeasController ideasController;


        [TestMethod]
        [TestCategory("Integration")]
        [ExcludeFromCodeCoverage]
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
            string workOrderId = EnsureRemedyWorkOrderCreated(newIdeaTitle);

            // Creating the Remedy Work Order should update the initiative with its Work Order Id
            await EnsureInitiativeUpdatedWithWorkOrderId(newInitiative.Id, workOrderId);

            // ensure the "RremedyChecker" verifies the Work Order was created, and send a message on the service bus
            // to notify Octava that the Work Order has been created
            var remedyChecker = serviceProvider.GetRequiredService<IntegrationRemedyChecker>();
            await remedyChecker.PollAsync(startTime.ToUniversalTime());
            Assert.AreEqual(1, remedyChecker.WorkOrdersProcessed.Where(x => x.WorkOrderId == workOrderId).Count(), $"Expected 1 new remedy item added since the test started at { startTime }");

            string baEmail = await SimulateWorkOrderAssignee(workOrderId);

            await EnsureBusinessAnalystAssigned(workOrderId, baEmail);
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

            Assert.IsTrue(createdIdea.Id > 0, "Idea created but not assigned an Id");

            return createdIdea;
        }

        private string EnsureRemedyWorkOrderCreated(string newInitiativeTitle)
        {
            // ensure the remedyNewIdeaService got the idea and posted a new Work Order in Remedy,
            // this should also send a message on the bus with the Remedy Work Order Id

            // we'll give 10 seconds to get the item from the service bus
            var timeout = new TimeSpan(0, 0, 10).TotalMilliseconds;
            Stopwatch watch = new Stopwatch();
            watch.Start();
            var remedyNewIdeaService = serviceProvider.GetRequiredService<IntegrationRemedyListenerNewIdeaListener>();

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

        private async Task EnsureInitiativeUpdatedWithWorkOrderId(long initiativeId, string workOrderId)
        {
            // we'll give 10 seconds to get the item from the service bus
            var timeout = new TimeSpan(0, 0, 10).TotalMilliseconds;
            Stopwatch watch = new Stopwatch();
            watch.Start();
            var listener = serviceProvider.GetRequiredService<IntegrationRemedyItemUpdatedIdeaListener>();

            while (true)
            {
                if (watch.ElapsedMilliseconds >= timeout)
                {
                    Assert.Fail("Timeout waiting for the Remedy ItemUpdatedListener to receive initiative updates");
                    break;
                }
                if (listener.WorkOrdersCreated.Any(x => x.WorkOrderId == workOrderId))
                    break;
                logger.Information("Waiting for Remedy ItemUpdatedListener...");
                Thread.Sleep(500); // wait half a second and try again
            }

            // get a fresh Initiative to ensure we have the latest changes
            var workOrderCreatedArgs = listener.WorkOrdersCreated.SingleOrDefault(x => x.WorkOrderId == workOrderId);
            Assert.IsNotNull(workOrderCreatedArgs, $"Unable to get the single WorkOrderCreatedArgs of the WorkOrderCreated event for WorkOrder { workOrderId }");
            Assert.IsNotNull(workOrderCreatedArgs.Initiative, "Expected Initiative of WorkOrderCreatedArgs to be non null");
            Assert.IsTrue(workOrderCreatedArgs.Initiative.Id > 0, $"Expected Initiative of WorkOrderCreatedArgs have a valid id, but got { workOrderCreatedArgs.Initiative.Id }");

            var initiativeRepository = serviceProvider.GetRequiredService<IIdeaRepository>();
            var initiative = await initiativeRepository.GetIdeaAsync(initiativeId);
            Assert.IsNotNull(initiative, $"Could not retrieve updated initiative with id { initiativeId }");

            Assert.AreEqual(workOrderId, initiative.WorkItemId, "Initiative was not updated with Remedy WorkOrder Id after the work order was created");

            Assert.AreEqual(InitiativeStatus.Submit, initiative.Status, "Expected initiative to be in Submit status");
        }

        private async Task<String> SimulateWorkOrderAssignee(string workOrderId)
        {
            var remedyChecker = serviceProvider.GetRequiredService<IntegrationRemedyChecker>();
            remedyChecker.WorkOrdersProcessed.Clear();
            await remedyChecker.SimlateBusinessAnalystAssigned(workOrderId, "debgus");
            var workOrderProcessed = remedyChecker.WorkOrdersProcessed.SingleOrDefault(x => x.WorkOrderId == workOrderId);
            Assert.IsNotNull(workOrderProcessed, "Unable to process simulated assignment of Business Analyst");
            Assert.IsTrue(!string.IsNullOrWhiteSpace(workOrderProcessed.AssigneeEmail), "Business Analyst was assigned but no email address was retrieved");
            return workOrderProcessed.AssigneeEmail;
        }

        private async Task EnsureBusinessAnalystAssigned(string workOrderId, string userEmail)
        {
            // we'll give 10 seconds to get the item from the service bus
            var timeout = new TimeSpan(0, 0, 10).TotalMilliseconds;
            Stopwatch watch = new Stopwatch();
            watch.Start();
            var listener = serviceProvider.GetRequiredService<IntegrationRemedyItemUpdatedIdeaListener>();

            while (true)
            {
                if (watch.ElapsedMilliseconds >= timeout)
                {
                    Assert.Fail("Timeout waiting for the Remedy ItemUpdatedListener to receive initiative updates");
                    break;
                }
                if (listener.WorkOrdersUpdated.Any(x => x.Item1.WorkOrderId == workOrderId && !string.IsNullOrWhiteSpace(x.Item1.AssigneeEmail)))
                    break;
                logger.Information("Waiting for Remedy ItemUpdatedListener...");
                Thread.Sleep(500); // wait half a second and try again
            }

            // get a fresh Initiative to ensure we have the latest changes
            var workOrderUpdatedArgs = listener.WorkOrdersUpdated.SingleOrDefault(x => x.Item1.WorkOrderId == workOrderId && !string.IsNullOrWhiteSpace(x.Item1.AssigneeEmail));
            Assert.IsNotNull(workOrderUpdatedArgs, $"Unable to get the single WorkOrderUpdatedrgs of the WorkOrderUpdated event for WorkOrder { workOrderId }");
            Assert.IsNotNull(workOrderUpdatedArgs.Item2, $"Unable to get initiative for WorkOrder { workOrderId }");
            Assert.IsTrue(workOrderUpdatedArgs.Item2.Id > 0, $"Expected Initiative of WorkOrderCreatedArgs have a valid id, but got { workOrderUpdatedArgs.Item2.Id }");

            var initiativeRepository = serviceProvider.GetRequiredService<IIdeaRepository>();
            var initiative = await initiativeRepository.GetIdeaAsync(workOrderUpdatedArgs.Item2.Id);
            Assert.IsNotNull(initiative, $"Could not retrieve updated initiative with id { workOrderUpdatedArgs.Item2.Id }");

            Assert.IsNotNull(initiative.Assignee, "Assignee was not assigned to Initiative");
            Assert.AreEqual(userEmail, initiative.Assignee.Email, "Initiative was not updated with Remedy WorkOrder Id after the work order was created");
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

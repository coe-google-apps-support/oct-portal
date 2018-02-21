using CoE.Ideas.Core;
using CoE.Ideas.Integration.Notification;
using CoE.Ideas.Remedy.SbListener;
using CoE.Ideas.Server.Controllers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
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
            var newIdeaTitle = "Test Idea " + DateTime.Now.ToString("MMM dd yyyy HH:mm:ss");
            var newIdea = new Idea()
            {
                Title = newIdeaTitle,
                Description = "Test Idea description"
            };
            var postIdeaResult = await ideasController.PostIdea(newIdea) as Microsoft.AspNetCore.Mvc.ObjectResult;
            Assert.IsNotNull(postIdeaResult, "Could not create initiative");
            Assert.AreEqual(201, postIdeaResult.StatusCode, "IdeasController did not report success. Message returned was: {0}", postIdeaResult.Value);


            // ensure the remedyNewIdeaService got the idea
            var timeout = new TimeSpan(0, 1, 0);
            DateTime startTime = DateTime.Now;
            var remedyNewIdeaService = serviceProvider.GetRequiredService<IntegrationRemedyListenerNewIdeaListener> ();
            while (true)
            {
                if (DateTime.Now.Subtract(startTime) > timeout)
                {
                    Assert.Fail("Timeout waiting for the Remedy NewIdeaListener to receive the new initiative");
                    break;
                }
                if (remedyNewIdeaService.NewInitiatives.Any())
                    break;
                logger.Information("Waiting for Remedy NewIdeaListener...");
                Thread.Sleep(500);
            }

            var remedyService = serviceProvider.GetRequiredService<IntegrationRemedyService>();
            var remedyAddedItem = (remedyService.WorkOrdersAdded.FirstOrDefault(x => x.Idea.Title == newIdeaTitle));
            Assert.IsNotNull(remedyAddedItem, "New initiative did not create a Work Order in Remedy");
            Assert.IsTrue(!string.IsNullOrWhiteSpace(remedyAddedItem.WorkOrderId), "Remedy did not assign an Id to the work order created by a new initiative");
            await remedyNewIdeaService.CloseAsync();
        }
    }
}

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Threading.Tasks;

namespace CoE.Ideas.Core.Tests
{
    [TestClass]
    public class InitiativeTests
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
                .ConfigureIdeaServicesInMemory()
                .BuildServiceProvider();
        }

        private static ServiceProvider serviceProvider;


        //[TestMethod]
        //public async Task CreateInitiative()
        //{
        //    var initiativeRepository = serviceProvider.GetRequiredService<IIdeaRepository>();
        //    var newInitiative = await initiativeRepository.AddIdeaAsync(new Idea()
        //    {
        //        Title = "Test Idea",
        //        Description = "Test creating initiatives"
        //    });
        //    Assert.IsTrue(newInitiative.Id > 0, "Could not create initiative");
        //}
    }
}

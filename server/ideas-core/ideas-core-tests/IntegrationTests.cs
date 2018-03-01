using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace CoE.Ideas.Core.Tests
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

            var services = new ServiceCollection();
            services.AddIdeaConfiguration(
                config.GetConnectionString("IdeaDatabase"),
                config["Ideas:WordPressUrl"],
                config.GetConnectionString("WordPressDatabase"),
                config.GetSection("WordPress"));


            services.AddAutoMapper();

            serviceProvider = services.BuildServiceProvider();
        }

        private static ServiceProvider serviceProvider;

        [TestMethod]
        public async Task ChangeIdeaStatus()
        {
            long initiativeId = 84;

            var initiativeRepository = serviceProvider.GetRequiredService<IUpdatableIdeaRepository>();
            var idea = await initiativeRepository.GetIdeaAsync(initiativeId);
            Assert.IsNotNull(idea, "Could not get idea with id " + initiativeId);

            await initiativeRepository.SetWorkItemStatusAsync(initiativeId, InitiativeStatus.Submit);

            var updatedIdea = await initiativeRepository.GetIdeaAsync(initiativeId);
        }
    }
}

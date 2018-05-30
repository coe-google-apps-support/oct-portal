using CoE.Ideas.Core.Data;
using CoE.Ideas.Core.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;

namespace CoE.Ideas.Core.Tests
{
    public class InitiativeTests
    {
        [SetUp]
        public void Setup()
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


        [Test]
        public async Task CreateInitiative()
        {
            var initiativeRepository = serviceProvider.GetRequiredService<IInitiativeRepository>();
            var newInitiative = await initiativeRepository.AddInitiativeAsync(Initiative.Create(
                 title: "Test Idea",
                 description: "Test creating initiatives",
                 ownerPersonId: 1
             ));

            newInitiative.Should().NotBeNull();
            newInitiative.Title.Should().Be("Test Idea");
        }
    }
}
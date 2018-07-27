using CoE.Ideas.Core.Data;
using CoE.Ideas.Core.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using CoE.Ideas.Core.ServiceBus;
using System.Collections.Generic;
using System.Linq;

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
                .AddInitiativeMessaging()
                .BuildServiceProvider();

        }

        private static ServiceProvider serviceProvider;


        [Test]
        public async Task CreateInitiative()
        {
            var newInitiativeMessages = new List<InitiativeCreatedEventArgs>();
            serviceProvider.GetRequiredService<SynchronousInitiativeMessageReceiver>()
                .CreatedHandlers.Add((e, token) => { newInitiativeMessages.Add(e); return Task.CompletedTask; });

            var initiativeRepository = serviceProvider.GetRequiredService<IInitiativeRepository>();

            // create a basic initiative
            var newInitiative = Initiative.Create(
                 title: "Test Idea",
                 description: "Test creating initiatives",
                 ownerPersonId: 1,
                 businessContactId: 2
             );

            // add some supporting documents
            newInitiative.AddSupportingDocument(
                SupportingDocument.Create("My Document", "www.edmonton.ca", SupportingDocumentsType.BusinessCases));
            newInitiative.AddSupportingDocument(
                SupportingDocument.Create("Document2", "github.com", SupportingDocumentsType.Other));

            // add some stakeholders
            newInitiative.AddStakeholder(3, StakeholderType.BusinessContact);

            var newInitiative2 = await initiativeRepository.AddInitiativeAsync(newInitiative);

            newInitiative2.Should().NotBeNull();
            newInitiative2.Title.Should().Be("Test Idea");
            newInitiative2.SupportingDocuments.Count().Should().Be(2);

            newInitiativeMessages.Count().Should().Be(1);
        }

    }
}
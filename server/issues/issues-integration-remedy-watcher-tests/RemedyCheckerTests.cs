using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;

namespace CoE.Issues.Remedy.Watcher.Tests
{
    public class RemedyCheckerTests
    {
        [SetUp]
        public void Setup()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.test.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            serviceProvider = new TestConfiguration(config)
                .ConfigureBasicServices()
                .AddMockIssueRepository()
                .AddMockRemedyService()
                .AddRemedyChecker()
                .BuildServiceProvider();
        }

        private static ServiceProvider serviceProvider;

        [Test]
        public void GetPollTimeTest()
        {
            IRemedyChecker checker = serviceProvider.GetRequiredService<IRemedyChecker>();
            DateTime actualTime = DateTimeOffset.Parse("2018-06-08T12:53:03-06:00").UtcDateTime;
            DateTime recentFile = checker.TryReadLastPollTime();
            recentFile.Should().Be(actualTime);
        }
        
        [Test]
        public async Task LogsRemedyServiceErrorsTest()
        {
            IRemedyChecker checker = serviceProvider.GetRequiredService<IRemedyChecker>();
            RemedyPollResult result = await checker.PollAsync(DateTime.Now);
            // Our mocked remedey service throws an exception, so it should show here.
            result.ProcessErrors.Should().NotBeEmpty();
        }
    }
}
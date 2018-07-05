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
                .BuildServiceProvider();
        }

        private static ServiceProvider serviceProvider;


        [Test]
        public void PollTest()
        {
            IRemedyChecker checker = serviceProvider.GetRequiredService<IRemedyChecker>();
        }

    }
}
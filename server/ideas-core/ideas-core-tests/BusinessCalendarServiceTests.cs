using CoE.Ideas.Core.Services;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System;
using System.IO;
using System.Threading.Tasks;

namespace CoE.Ideas.Core.Tests
{
    public class BusinessCalendarServiceTests
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
                .ConfigureBusinessCalendarService()
                .BuildServiceProvider();

        }

        private static ServiceProvider serviceProvider;

        [Test]
        public async Task BusinessCalendarServiceWithinSameDay()
        {
            var svc = serviceProvider.GetRequiredService<IBusinessCalendarService>();
            var statusUpdateTime = new DateTime(2018, 3, 23, 8, 0, 0);
            TimeSpan addTime = new TimeSpan(4, 0, 0);
            var eta = await svc.AddBusinessTime(statusUpdateTime, addTime);
            eta.Should().Be(statusUpdateTime.AddHours(4), because: "Expected ETA was not the correct value");
        }

        [Test]
        public async Task BusinessCalendarServiceToNextDay()
        {
            var svc = serviceProvider.GetRequiredService<IBusinessCalendarService>();
            var statusUpdateTime = new DateTime(2018, 3, 23, 16, 30, 0);
            TimeSpan addTime = new TimeSpan(4, 0, 0);
            var eta = await svc.AddBusinessTime(statusUpdateTime, addTime);
            eta.Should().Be(new DateTime(2018, 3, 26, 10, 30, 0), because: "Expected ETA was not the correct value");
        }

        [Test]
        public async Task BusinessCalendarServiceOverLongWeekend()
        {
            var svc = serviceProvider.GetRequiredService<IBusinessCalendarService>();
            var statusUpdateTime = new DateTime(2018, 3, 29, 14, 45, 20);
            TimeSpan addTime = new TimeSpan(4, 0, 0);
            var eta = await svc.AddBusinessTime(statusUpdateTime, addTime);
            eta.Should().Be(new DateTime(2018, 4, 3, 8, 45, 20), because: "Expected ETA was not the correct value");
        }

        [Test]
        public async Task BusinessCalendarServiceStartingBeforeWorkHours()
        {
            var svc = serviceProvider.GetRequiredService<IBusinessCalendarService>();
            var statusUpdateTime = new DateTime(2018, 3, 23, 3, 45, 20);
            TimeSpan addTime = new TimeSpan(4, 0, 0);
            var eta = await svc.AddBusinessTime(statusUpdateTime, addTime);
            eta.Should().Be(new DateTime(2018, 3, 23, 11, 0, 0), because: "Expected ETA was not the correct value");
        }

        [Test]
        public async Task BusinessCalendarServiceDaysNextWeekday()
        {
            var svc = serviceProvider.GetRequiredService<IBusinessCalendarService>();
            var statusUpdateTime = new DateTime(2018, 3, 21, 13, 45, 20);
            var eta = await svc.AddBusinessDays(statusUpdateTime, 2);
            eta.Should().Be(statusUpdateTime.AddDays(2), because: "Expected ETA was not the correct value");
        }

        [Test]
        public async Task BusinessCalendarServiceDaysOverWeekend()
        {
            var svc = serviceProvider.GetRequiredService<IBusinessCalendarService>();
            var statusUpdateTime = new DateTime(2018, 3, 23, 12, 0, 0);
            var eta = await svc.AddBusinessDays(statusUpdateTime, 3);
            eta.Should().Be(new DateTime(2018, 3, 28, 12, 0, 0), because: "Expected ETA was not the correct value");
        }

        [Test]
        public async Task BusinessCalendarServiceDaysBeforeWorkHours()
        {
            var svc = serviceProvider.GetRequiredService<IBusinessCalendarService>();
            var statusUpdateTime = new DateTime(2018, 3, 22, 4, 20, 0);
            var eta = await svc.AddBusinessDays(statusUpdateTime, 3);
            eta.Should().Be(new DateTime(2018, 3, 27, 7, 0, 0), because: "Expected ETA was not the correct value");
        }

        [Test]
        public async Task BusinessCalendarServiceOver1Month()
        {
            var svc = serviceProvider.GetRequiredService<IBusinessCalendarService>();
            var statusUpdateTime = new DateTime(2018, 3, 23, 3, 45, 20);
            var eta = await svc.AddBusinessDays(statusUpdateTime, 40);
            eta.Should().Be(new DateTime(2018, 5, 23, 7, 0, 0), because: "Expected ETA was not the correct value");
        }
    }
}

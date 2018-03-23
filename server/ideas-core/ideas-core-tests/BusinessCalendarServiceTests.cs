using CoE.Ideas.Core.Data;
using CoE.Ideas.Core.Services;
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
    public class BusinessCalendarServiceTests
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
                .ConfigureBusinessCalendarService()
                .BuildServiceProvider();
        }

        private static ServiceProvider serviceProvider;


        [TestMethod]
        public async Task BusinessCalendarServiceWithinSameDay()
        {
            var svc = serviceProvider.GetRequiredService<IBusinessCalendarService>();
            var statusUpdateTime = new DateTime(2018, 3, 23, 8, 0, 0);
            TimeSpan addTime = new TimeSpan(4, 0, 0);
            var eta = await svc.AddBusinessTime(statusUpdateTime, addTime);
            Assert.AreEqual(statusUpdateTime.AddHours(4), eta, "Expected ETA was not the correct value");
        }

        [TestMethod]
        public async Task BusinessCalendarServiceToNextDay()
        {
            var svc = serviceProvider.GetRequiredService<IBusinessCalendarService>();
            var statusUpdateTime = new DateTime(2018, 3, 23, 16, 30, 0);
            TimeSpan addTime = new TimeSpan(4, 0, 0);
            var eta = await svc.AddBusinessTime(statusUpdateTime, addTime);
            Assert.AreEqual(new DateTime(2018, 3, 26, 10, 30,0), eta, "Expected ETA was not the correct value");
        }

        [TestMethod]
        public async Task BusinessCalendarServiceOverLongWeekend()
        {
            var svc = serviceProvider.GetRequiredService<IBusinessCalendarService>();
            var statusUpdateTime = new DateTime(2018, 3, 29, 14, 45, 20);
            TimeSpan addTime = new TimeSpan(4, 0, 0);
            var eta = await svc.AddBusinessTime(statusUpdateTime, addTime);
            Assert.AreEqual(new DateTime(2018, 4, 3, 8, 45, 20), eta, "Expected ETA was not the correct value");
        }

        [TestMethod]
        public async Task BusinessCalendarServiceStartingBeforeWorkHours()
        {
            var svc = serviceProvider.GetRequiredService<IBusinessCalendarService>();
            var statusUpdateTime = new DateTime(2018, 3, 23, 3, 45, 20);
            TimeSpan addTime = new TimeSpan(4, 0, 0);
            var eta = await svc.AddBusinessTime(statusUpdateTime, addTime);
            Assert.AreEqual(new DateTime(2018, 3, 23, 11, 0, 0), eta, "Expected ETA was not the correct value");
        }

        [TestMethod]
        public async Task BusinessCalendarServiceDaysNextWeekday()
        {
            var svc = serviceProvider.GetRequiredService<IBusinessCalendarService>();
            var statusUpdateTime = new DateTime(2018, 3, 21, 13, 45, 20);
            var eta = await svc.AddBusinessDays(statusUpdateTime, 2);
            Assert.AreEqual(statusUpdateTime.AddDays(2), eta, "Expected ETA was not the correct value");
        }

        [TestMethod]
        public async Task BusinessCalendarServiceDaysOverWeekend()
        {
            var svc = serviceProvider.GetRequiredService<IBusinessCalendarService>();
            var statusUpdateTime = new DateTime(2018, 3, 23, 12, 0, 0);
            var eta = await svc.AddBusinessDays(statusUpdateTime, 3);
            Assert.AreEqual(new DateTime(2018, 3, 28, 12, 0, 0), eta, "Expected ETA was not the correct value");
        }

        [TestMethod]
        public async Task BusinessCalendarServiceDaysBeforeWorkHours()
        {
            var svc = serviceProvider.GetRequiredService<IBusinessCalendarService>();
            var statusUpdateTime = new DateTime(2018, 3, 22, 4, 20, 0);
            var eta = await svc.AddBusinessDays(statusUpdateTime, 3);
            Assert.AreEqual(new DateTime(2018, 3, 27, 7, 0, 0), eta, "Expected ETA was not the correct value");
        }

        [TestMethod]
        public async Task BusinessCalendarServiceOver1Month()
        {
            var svc = serviceProvider.GetRequiredService<IBusinessCalendarService>();
            var statusUpdateTime = new DateTime(2018, 3, 23, 3, 45, 20);
            var eta = await svc.AddBusinessDays(statusUpdateTime, 40);
            Assert.AreEqual(new DateTime(2018, 5, 23, 7, 0, 0), eta, "Expected ETA was not the correct value");
        }
    }
}

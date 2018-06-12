using CoE.Ideas.Core.Services;
using DateTimeExtensions.WorkingDays;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System;
using System.IO;
using System.Linq;
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
        public void TestCalendarHolidays()
        {
            var svc = serviceProvider.GetRequiredService<IHolidayStrategy>();
            var holidays = svc.Holidays;
            holidays.Should().ContainSingle(x => x.Name == "New Year");
            holidays.Should().ContainSingle(x => x.Name == "Family Day");
            holidays.Should().ContainSingle(x => x.Name == "Good Friday");
            holidays.Should().ContainSingle(x => x.Name == "Easter Monday");
            holidays.Should().ContainSingle(x => x.Name == "Victoria Day");
            holidays.Should().ContainSingle(x => x.Name == "Canada Day");
            holidays.Should().ContainSingle(x => x.Name == "Civic Holiday");
            holidays.Should().ContainSingle(x => x.Name == "Labour Day");
            holidays.Should().ContainSingle(x => x.Name == "Thanksgiving");
            holidays.Should().ContainSingle(x => x.Name == "Remembrance Day");
            holidays.Should().ContainSingle(x => x.Name == "Christmas");
            holidays.Should().ContainSingle(x => x.Name == "Boxing Day");

            // dates from http://www.kaldix.com/canada/calendar/alberta/holidays/

            // New Year's Day
            var newYears = holidays.Single(x => x.Name == "New Year");
            newYears.GetInstance(2012).Should().Be(new DateTime(2012, 1, 2)); // Sunday-> Monday
            newYears.GetInstance(2013).Should().Be(new DateTime(2013, 1, 1)); // Tuesday
            newYears.GetInstance(2014).Should().Be(new DateTime(2014, 1, 1)); // Wednesday
            newYears.GetInstance(2015).Should().Be(new DateTime(2015, 1, 1)); // Thursday
            newYears.GetInstance(2016).Should().Be(new DateTime(2016, 1, 1)); // Friday
            newYears.GetInstance(2017).Should().Be(new DateTime(2017, 1, 2)); // Sunday -> Monday
            newYears.GetInstance(2018).Should().Be(new DateTime(2018, 1, 1)); // Monday
            newYears.GetInstance(2019).Should().Be(new DateTime(2019, 1, 1)); // Tuesday
            newYears.GetInstance(2020).Should().Be(new DateTime(2020, 1, 1)); // Wednesday
            newYears.GetInstance(2021).Should().Be(new DateTime(2021, 1, 1)); // Friday
            newYears.GetInstance(2022).Should().Be(new DateTime(2022, 1, 3)); // Saturday -> Monday
            newYears.GetInstance(2023).Should().Be(new DateTime(2023, 1, 2)); // Sunday -> Monday
            newYears.GetInstance(2024).Should().Be(new DateTime(2024, 1, 1)); // Monday
            newYears.GetInstance(2025).Should().Be(new DateTime(2025, 1, 1)); // Wednesday

            // Family Day
            var familyDay = holidays.Single(x => x.Name == "Family Day");
            familyDay.GetInstance(2012).Should().Be(new DateTime(2012, 2, 20));
            familyDay.GetInstance(2013).Should().Be(new DateTime(2013, 2, 18));
            familyDay.GetInstance(2014).Should().Be(new DateTime(2014, 2, 17));
            familyDay.GetInstance(2015).Should().Be(new DateTime(2015, 2, 16));
            familyDay.GetInstance(2016).Should().Be(new DateTime(2016, 2, 15));
            familyDay.GetInstance(2017).Should().Be(new DateTime(2017, 2, 20));
            familyDay.GetInstance(2018).Should().Be(new DateTime(2018, 2, 19));
            familyDay.GetInstance(2019).Should().Be(new DateTime(2019, 2, 18));
            familyDay.GetInstance(2020).Should().Be(new DateTime(2020, 2, 17));
            familyDay.GetInstance(2021).Should().Be(new DateTime(2021, 2, 15));
            familyDay.GetInstance(2022).Should().Be(new DateTime(2022, 2, 21));
            familyDay.GetInstance(2023).Should().Be(new DateTime(2023, 2, 20));
            familyDay.GetInstance(2024).Should().Be(new DateTime(2024, 2, 19));
            familyDay.GetInstance(2025).Should().Be(new DateTime(2025, 2, 17));
            familyDay.GetInstance(2026).Should().Be(new DateTime(2026, 2, 16));
            familyDay.GetInstance(2027).Should().Be(new DateTime(2027, 2, 15));

            // Good Friday
            var goodFriday = holidays.Single(x => x.Name == "Good Friday");
            goodFriday.GetInstance(2012).Should().Be(new DateTime(2012, 4, 6));
            goodFriday.GetInstance(2013).Should().Be(new DateTime(2013, 3, 29));
            goodFriday.GetInstance(2014).Should().Be(new DateTime(2014, 4, 18));
            goodFriday.GetInstance(2015).Should().Be(new DateTime(2015, 4, 3));
            goodFriday.GetInstance(2016).Should().Be(new DateTime(2016, 3, 25));
            goodFriday.GetInstance(2017).Should().Be(new DateTime(2017, 4, 14));
            goodFriday.GetInstance(2018).Should().Be(new DateTime(2018, 3, 30));
            goodFriday.GetInstance(2019).Should().Be(new DateTime(2019, 4, 19));
            goodFriday.GetInstance(2020).Should().Be(new DateTime(2020, 4, 10));
            goodFriday.GetInstance(2021).Should().Be(new DateTime(2021, 4, 2));
            goodFriday.GetInstance(2022).Should().Be(new DateTime(2022, 4, 15));
            goodFriday.GetInstance(2023).Should().Be(new DateTime(2023, 4, 7));
            goodFriday.GetInstance(2024).Should().Be(new DateTime(2024, 3, 29));
            goodFriday.GetInstance(2025).Should().Be(new DateTime(2025, 4, 18));
            goodFriday.GetInstance(2026).Should().Be(new DateTime(2026, 4, 3));
            goodFriday.GetInstance(2027).Should().Be(new DateTime(2027, 3, 26));

            // Easter Monday
            var easterMonday = holidays.Single(x => x.Name == "Easter Monday");
            easterMonday.GetInstance(2012).Should().Be(new DateTime(2012, 4, 9));
            easterMonday.GetInstance(2013).Should().Be(new DateTime(2013, 4, 1));
            easterMonday.GetInstance(2014).Should().Be(new DateTime(2014, 4, 21));
            easterMonday.GetInstance(2015).Should().Be(new DateTime(2015, 4, 6));
            easterMonday.GetInstance(2016).Should().Be(new DateTime(2016, 3, 28));
            easterMonday.GetInstance(2017).Should().Be(new DateTime(2017, 4, 17));
            easterMonday.GetInstance(2018).Should().Be(new DateTime(2018, 4, 2));
            easterMonday.GetInstance(2019).Should().Be(new DateTime(2019, 4, 22));
            easterMonday.GetInstance(2020).Should().Be(new DateTime(2020, 4, 13));
            easterMonday.GetInstance(2021).Should().Be(new DateTime(2021, 4, 5));
            easterMonday.GetInstance(2022).Should().Be(new DateTime(2022, 4, 18));
            easterMonday.GetInstance(2023).Should().Be(new DateTime(2023, 4, 10));
            easterMonday.GetInstance(2024).Should().Be(new DateTime(2024, 4, 1));
            easterMonday.GetInstance(2025).Should().Be(new DateTime(2025, 4, 21));
            easterMonday.GetInstance(2026).Should().Be(new DateTime(2026, 4, 6));
            easterMonday.GetInstance(2027).Should().Be(new DateTime(2027, 3, 29));

            // Victoria Day
            var victoriaDay = holidays.Single(x => x.Name == "Victoria Day");
            victoriaDay.GetInstance(2012).Should().Be(new DateTime(2012, 5, 21));
            victoriaDay.GetInstance(2013).Should().Be(new DateTime(2013, 5, 20));
            victoriaDay.GetInstance(2014).Should().Be(new DateTime(2014, 5, 19));
            victoriaDay.GetInstance(2015).Should().Be(new DateTime(2015, 5, 18));
            victoriaDay.GetInstance(2016).Should().Be(new DateTime(2016, 5, 23));
            victoriaDay.GetInstance(2017).Should().Be(new DateTime(2017, 5, 22));
            victoriaDay.GetInstance(2018).Should().Be(new DateTime(2018, 5, 21));
            victoriaDay.GetInstance(2019).Should().Be(new DateTime(2019, 5, 20));
            victoriaDay.GetInstance(2020).Should().Be(new DateTime(2020, 5, 18));
            victoriaDay.GetInstance(2021).Should().Be(new DateTime(2021, 5, 24));
            victoriaDay.GetInstance(2022).Should().Be(new DateTime(2022, 5, 23));
            victoriaDay.GetInstance(2023).Should().Be(new DateTime(2023, 5, 22));
            victoriaDay.GetInstance(2024).Should().Be(new DateTime(2024, 5, 20));
            victoriaDay.GetInstance(2025).Should().Be(new DateTime(2025, 5, 19));
            victoriaDay.GetInstance(2026).Should().Be(new DateTime(2026, 5, 18));
            victoriaDay.GetInstance(2027).Should().Be(new DateTime(2027, 5, 24));

            // Canada Day
            var canadaDay = holidays.Single(x => x.Name == "Canada Day");
            canadaDay.GetInstance(2012).Should().Be(new DateTime(2012, 7, 2)); // was on a sunday in 2012
            canadaDay.GetInstance(2013).Should().Be(new DateTime(2013, 7, 1));
            canadaDay.GetInstance(2014).Should().Be(new DateTime(2014, 7, 1));
            canadaDay.GetInstance(2015).Should().Be(new DateTime(2015, 7, 1));
            canadaDay.GetInstance(2016).Should().Be(new DateTime(2016, 7, 1));
            canadaDay.GetInstance(2017).Should().Be(new DateTime(2017, 7, 3));  // saturday
            canadaDay.GetInstance(2018).Should().Be(new DateTime(2018, 7, 2));
            canadaDay.GetInstance(2019).Should().Be(new DateTime(2019, 7, 1));
            canadaDay.GetInstance(2020).Should().Be(new DateTime(2020, 7, 1));
            canadaDay.GetInstance(2021).Should().Be(new DateTime(2021, 7, 1));
            canadaDay.GetInstance(2022).Should().Be(new DateTime(2022, 7, 1));
            canadaDay.GetInstance(2023).Should().Be(new DateTime(2023, 7, 3));
            canadaDay.GetInstance(2024).Should().Be(new DateTime(2024, 7, 1));
            canadaDay.GetInstance(2025).Should().Be(new DateTime(2025, 7, 1));
            canadaDay.GetInstance(2026).Should().Be(new DateTime(2026, 7, 1));
            canadaDay.GetInstance(2027).Should().Be(new DateTime(2027, 7, 1));

            // Labour Day
            var labourDay = holidays.Single(x => x.Name == "Labour Day");
            labourDay.GetInstance(2012).Should().Be(new DateTime(2012, 9, 3));
            labourDay.GetInstance(2013).Should().Be(new DateTime(2013, 9, 2));
            labourDay.GetInstance(2014).Should().Be(new DateTime(2014, 9, 1));
            labourDay.GetInstance(2015).Should().Be(new DateTime(2015, 9, 7));
            labourDay.GetInstance(2016).Should().Be(new DateTime(2016, 9, 5));
            labourDay.GetInstance(2017).Should().Be(new DateTime(2017, 9, 4));
            labourDay.GetInstance(2018).Should().Be(new DateTime(2018, 9, 3));
            labourDay.GetInstance(2019).Should().Be(new DateTime(2019, 9, 2));
            labourDay.GetInstance(2020).Should().Be(new DateTime(2020, 9, 7));
            labourDay.GetInstance(2021).Should().Be(new DateTime(2021, 9, 6));
            labourDay.GetInstance(2022).Should().Be(new DateTime(2022, 9, 5));
            labourDay.GetInstance(2023).Should().Be(new DateTime(2023, 9, 4));
            labourDay.GetInstance(2024).Should().Be(new DateTime(2024, 9, 2));
            labourDay.GetInstance(2025).Should().Be(new DateTime(2025, 9, 1));
            labourDay.GetInstance(2026).Should().Be(new DateTime(2026, 9, 7));
            labourDay.GetInstance(2027).Should().Be(new DateTime(2027, 9, 6));

            // Thanksgiving
            var thanksgiving = holidays.Single(x => x.Name == "Thanksgiving");
            thanksgiving.GetInstance(2012).Should().Be(new DateTime(2012, 10, 8));
            thanksgiving.GetInstance(2013).Should().Be(new DateTime(2013, 10, 14));
            thanksgiving.GetInstance(2014).Should().Be(new DateTime(2014, 10, 13));
            thanksgiving.GetInstance(2015).Should().Be(new DateTime(2015, 10, 12));
            thanksgiving.GetInstance(2016).Should().Be(new DateTime(2016, 10, 10));
            thanksgiving.GetInstance(2017).Should().Be(new DateTime(2017, 10, 9));
            thanksgiving.GetInstance(2018).Should().Be(new DateTime(2018, 10, 8));
            thanksgiving.GetInstance(2019).Should().Be(new DateTime(2019, 10, 14));
            thanksgiving.GetInstance(2020).Should().Be(new DateTime(2020, 10, 12));
            thanksgiving.GetInstance(2021).Should().Be(new DateTime(2021, 10, 11));
            thanksgiving.GetInstance(2022).Should().Be(new DateTime(2022, 10, 10));
            thanksgiving.GetInstance(2023).Should().Be(new DateTime(2023, 10, 9));
            thanksgiving.GetInstance(2024).Should().Be(new DateTime(2024, 10, 14));
            thanksgiving.GetInstance(2025).Should().Be(new DateTime(2025, 10, 13));
            thanksgiving.GetInstance(2026).Should().Be(new DateTime(2026, 10, 12));
            thanksgiving.GetInstance(2027).Should().Be(new DateTime(2027, 10, 11));

            // Remembrance Day
            var remembranceDay = holidays.Single(x => x.Name == "Remembrance Day");
            remembranceDay.GetInstance(2012).Should().Be(new DateTime(2012, 11, 12)); // was on a sunday in 2012
            remembranceDay.GetInstance(2013).Should().Be(new DateTime(2013, 11, 11));
            remembranceDay.GetInstance(2014).Should().Be(new DateTime(2014, 11, 11));
            remembranceDay.GetInstance(2015).Should().Be(new DateTime(2015, 11, 11));
            remembranceDay.GetInstance(2016).Should().Be(new DateTime(2016, 11, 11));
            remembranceDay.GetInstance(2017).Should().Be(new DateTime(2017, 11, 13)); // saturday
            remembranceDay.GetInstance(2018).Should().Be(new DateTime(2018, 11, 12)); // sunday
            remembranceDay.GetInstance(2019).Should().Be(new DateTime(2019, 11, 11));
            remembranceDay.GetInstance(2020).Should().Be(new DateTime(2020, 11, 11));
            remembranceDay.GetInstance(2021).Should().Be(new DateTime(2021, 11, 11));
            remembranceDay.GetInstance(2022).Should().Be(new DateTime(2022, 11, 11));
            remembranceDay.GetInstance(2023).Should().Be(new DateTime(2023, 11, 13)); // saturday
            remembranceDay.GetInstance(2024).Should().Be(new DateTime(2024, 11, 11));
            remembranceDay.GetInstance(2025).Should().Be(new DateTime(2025, 11, 11));
            remembranceDay.GetInstance(2026).Should().Be(new DateTime(2026, 11, 11));
            remembranceDay.GetInstance(2027).Should().Be(new DateTime(2027, 11, 11));

            // Christmas
            var christmas = holidays.Single(x => x.Name == "Christmas");
            christmas.GetInstance(2012).Should().Be(new DateTime(2012, 12, 25));
            christmas.GetInstance(2013).Should().Be(new DateTime(2013, 12, 25));
            christmas.GetInstance(2014).Should().Be(new DateTime(2014, 12, 25));
            christmas.GetInstance(2015).Should().Be(new DateTime(2015, 12, 25));
            christmas.GetInstance(2016).Should().Be(new DateTime(2016, 12, 26)); // Sunday
            christmas.GetInstance(2017).Should().Be(new DateTime(2017, 12, 25));
            christmas.GetInstance(2018).Should().Be(new DateTime(2018, 12, 25));
            christmas.GetInstance(2019).Should().Be(new DateTime(2019, 12, 25));
            christmas.GetInstance(2020).Should().Be(new DateTime(2020, 12, 25));
            christmas.GetInstance(2021).Should().Be(new DateTime(2021, 12, 27)); // saturday
            christmas.GetInstance(2022).Should().Be(new DateTime(2022, 12, 26)); // sunday
            christmas.GetInstance(2023).Should().Be(new DateTime(2023, 12, 25));
            christmas.GetInstance(2024).Should().Be(new DateTime(2024, 12, 25));
            christmas.GetInstance(2025).Should().Be(new DateTime(2025, 12, 25));
            christmas.GetInstance(2026).Should().Be(new DateTime(2026, 12, 25));
            christmas.GetInstance(2027).Should().Be(new DateTime(2027, 12, 27)); // saturday

            // Boxing Day
            var boxingDay = holidays.Single(x => x.Name == "Boxing Day");
            boxingDay.GetInstance(2012).Should().Be(new DateTime(2012, 12, 26));
            boxingDay.GetInstance(2013).Should().Be(new DateTime(2013, 12, 26));
            boxingDay.GetInstance(2014).Should().Be(new DateTime(2014, 12, 26));
            boxingDay.GetInstance(2015).Should().Be(new DateTime(2015, 12, 28)); // on saturday
            boxingDay.GetInstance(2016).Should().Be(new DateTime(2016, 12, 26));
            boxingDay.GetInstance(2017).Should().Be(new DateTime(2017, 12, 26));
            boxingDay.GetInstance(2018).Should().Be(new DateTime(2018, 12, 26));
            boxingDay.GetInstance(2019).Should().Be(new DateTime(2019, 12, 26));
            boxingDay.GetInstance(2020).Should().Be(new DateTime(2020, 12, 28)); // saturday
            boxingDay.GetInstance(2021).Should().Be(new DateTime(2021, 12, 27)); // sunday
            boxingDay.GetInstance(2022).Should().Be(new DateTime(2022, 12, 26));
            boxingDay.GetInstance(2023).Should().Be(new DateTime(2023, 12, 26));
            boxingDay.GetInstance(2024).Should().Be(new DateTime(2024, 12, 26));
            boxingDay.GetInstance(2025).Should().Be(new DateTime(2025, 12, 26));
            boxingDay.GetInstance(2026).Should().Be(new DateTime(2026, 12, 28)); // saturday
            boxingDay.GetInstance(2027).Should().Be(new DateTime(2027, 12, 27)); // sunday

        }


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

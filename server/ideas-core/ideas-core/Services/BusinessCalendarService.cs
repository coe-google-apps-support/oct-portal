using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using CoE.Ideas.Core.Data;
using EnsureThat;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CoE.Ideas.Core.Services
{
    internal class BusinessCalendarService : IBusinessCalendarService
    {
        public BusinessCalendarService(IOptions<BusinessCalendarServiceOptions> options)
        {
            EnsureArg.IsNotNull(options);
            EnsureArg.IsNotNull(options.Value);
            EnsureArg.IsNotNullOrWhiteSpace(options.Value.PayrollCalenderServiceUrl);
            var url = options.Value.PayrollCalenderServiceUrl;
            if (!url.EndsWith("/"))
                url = url + "/";
            _payrollServiceUrl = new Uri(url);

        }

        private readonly Uri _payrollServiceUrl;

        internal static TimeSpan StartBusinessHoursTime = new TimeSpan(7, 0, 0);
        internal static TimeSpan EndBusinessHoursTime = new TimeSpan(17, 0, 0);

        private ICollection<DateTime> workingDays = new HashSet<DateTime>();
        private DateTime firstWorkingDayLoaded = DateTime.MaxValue;
        private DateTime lastWorkingDayLoaded = DateTime.MinValue;
        protected virtual async Task<IEnumerable<DateTime>> GetWorkingDaysAsync(DateTime startDate)
        {
            DateTime startDateDay = startDate.Date;
            if (startDateDay < firstWorkingDayLoaded || startDateDay >= lastWorkingDayLoaded)
            {
                // load more working days
                using (var client = GetHttpClient())
                {
                    var thisMonthData = await client.GetStringAsync($"Month/{startDateDay.ToString("yyyy-MM-dd")}/Holidays");
                    var nextMonthDataTask = client.GetStringAsync($"Month/{startDateDay.AddMonths(1).ToString("yyyy-MM-dd")}/Holidays");
                    workingDays.Clear();
                    var addWorkingDays = new Action<DateTime, string>((monthStartDate, holidayData) =>
                    {
                        int monthNumber = monthStartDate.Month;
                        var holidayDayNumbers = new List<int>();
                        var holidays = JsonConvert.DeserializeObject<IEnumerable<HolidayData>>(holidayData);/* (System.Collections.IEnumerable)JObject.Parse(holidayData);*/
                        foreach (var d in holidays)
                        {
                            DateTime hd = d.HolidayDate;   // Not sure if this is gonna work...
                            holidayDayNumbers.Add(hd.Day);
                        }
                        for (DateTime d = monthStartDate; d.Month == monthNumber; d=d.AddDays(1))
                        {
                            if (d.DayOfWeek == DayOfWeek.Saturday || d.DayOfWeek == DayOfWeek.Sunday)
                                continue;
                            if (holidayDayNumbers.Contains(d.Day))
                                continue;
                            workingDays.Add(d.Date);
                        }
                    });
                    addWorkingDays(startDateDay, thisMonthData);
                    addWorkingDays(startDateDay.AddMonths(1).AddDays(-startDateDay.Day+1), await nextMonthDataTask);
                }
                if (workingDays.Any())
                {
                    firstWorkingDayLoaded = workingDays.First();
                    lastWorkingDayLoaded = workingDays.Last();
                }
            }

            return workingDays.SkipWhile(x => x.Date < startDateDay);
        }
        protected virtual async Task<DateTime> GetNextWorkingDayAsync(DateTime date)
        {
            return (await GetWorkingDaysAsync(date)).Skip(1).First().Date;
        }

        protected virtual HttpClient GetHttpClient()
        {

            var client = new HttpClient
            {
                BaseAddress = _payrollServiceUrl
            };

            // easy - no credentials to set :)
            return client;
        }

        public async Task<DateTime> AddBusinessTime(DateTime startTime, TimeSpan time)
        {
            if (time.Ticks < 0)
                return await Subtract(startTime, TimeSpan.FromTicks(Math.Abs(time.Ticks)));

            long ticksToAdd = time.Ticks;

            DateTime currentTime = startTime;

            // fix if time is before start hours time
            if (currentTime.Subtract(currentTime.Date) < StartBusinessHoursTime)
                currentTime = new DateTime(currentTime.Year, currentTime.Month, currentTime.Day).Add(StartBusinessHoursTime);
            // fix if time is after end hours time
            if (currentTime.Subtract(currentTime.Date) > EndBusinessHoursTime)
                currentTime = new DateTime(currentTime.Year, currentTime.Month, currentTime.Day+1).Add(StartBusinessHoursTime);

            DateTime eta = currentTime.AddTicks(ticksToAdd);
            var amountPastBusinessDay = eta.Subtract(currentTime.Date.Add(EndBusinessHoursTime));
            while (amountPastBusinessDay.Ticks > 0)
            {
                var nextWorkingDay = await GetNextWorkingDayAsync(currentTime);

                currentTime = nextWorkingDay.Date.Add(StartBusinessHoursTime);
                eta = currentTime.Add(amountPastBusinessDay);
                amountPastBusinessDay = eta.Subtract(currentTime.Date.Add(EndBusinessHoursTime));
            }
            return eta;
        }

        private Task<DateTime> Subtract(DateTime startTime, TimeSpan time)
        {
            throw new NotSupportedException("Currently subtracting business time is not supported.");
        }


        public async Task<DateTime> AddBusinessDays(DateTime startTime, int days)
        {
            if (days < 0)
                return await SubtractBusinessDays(startTime, Math.Abs(days));

            var currentDate = startTime;
            var timeComponent = currentDate.Subtract(currentDate.Date);
            if (timeComponent < StartBusinessHoursTime)
                timeComponent = StartBusinessHoursTime;
            for (int i=0; i<days; i++)
            {
                currentDate = await GetNextWorkingDayAsync(currentDate);
            }

            return currentDate.Add(timeComponent);
        }

        private Task<DateTime> SubtractBusinessDays(DateTime startTime, int v)
        {
            throw new NotSupportedException("Currently subtracting business days is not supported.");
        }

        private class HolidayData
        {
            public DateTime HolidayDate { get; set; }
        }

    }
}

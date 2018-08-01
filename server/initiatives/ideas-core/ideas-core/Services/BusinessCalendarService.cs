using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using CoE.Ideas.Core.Data;
using DateTimeExtensions;
using DateTimeExtensions.WorkingDays;
using EnsureThat;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CoE.Ideas.Core.Services
{
    internal class BusinessCalendarService : IBusinessCalendarService, IHolidayStrategy
    {
        public BusinessCalendarService(
            IOptions<BusinessCalendarServiceOptions> options,
            IMemoryCache cache)
        {
            EnsureArg.IsNotNull(options);
            EnsureArg.IsNotNull(options.Value);
            EnsureArg.IsNotNull(cache);
            StartBusinessHoursTime = options.Value.StartBusinessHoursTime;
            EndBusinessHoursTime = options.Value.EndBusinessHoursTime;
            _cache = cache;

            _workingDayCultureInfo = new WorkingDayCultureInfo()
            {
                LocateHolidayStrategy = (x,y) => this
            };

        }

        private readonly IWorkingDayCultureInfo _workingDayCultureInfo;
        private readonly TimeSpan StartBusinessHoursTime;
        private readonly TimeSpan EndBusinessHoursTime;

        private static IEnumerable<Holiday> _holidays;
        public IEnumerable<Holiday> Holidays
        {
            get
            {
                if (_holidays == null)
                {
                    var holidayList = new List<Holiday>
                    {
                        new NonWeekendHoliday(GlobalHolidays.NewYear),
                        new NthDayOfWeekInMonthHoliday("Family Day", 3, DayOfWeek.Monday, 2, CountDirection.FromFirst),
                        ChristianHolidays.GoodFriday,
                        ChristianHolidays.EasterMonday,
                        new VictoriaDay(),
                        new NonWeekendHoliday(new FixedHoliday("Canada Day", 7, 1)),
                        new NthDayOfWeekInMonthHoliday("Civic Holiday", 1, DayOfWeek.Monday, 8, CountDirection.FromFirst),
                        new NthDayOfWeekInMonthHoliday("Labour Day", 1, DayOfWeek.Monday, 9, CountDirection.FromFirst),
                        new NthDayOfWeekInMonthHoliday("Thanksgiving", 2, DayOfWeek.Monday, 10, CountDirection.FromFirst),
                        new NonWeekendHoliday(new FixedHoliday("Remembrance Day", 11, 11)),
                        new NonWeekendHoliday(ChristianHolidays.Christmas),
                        new NonWeekendHoliday(GlobalHolidays.BoxingDay)
                    };
                    _holidays = holidayList;
                }
                return _holidays;
            }
        }
        public IEnumerable<Holiday> GetHolidaysOfYear(int year)
        {
            throw new NotImplementedException();
        }

        private readonly IMemoryCache _cache;

        public bool IsHoliDay(DateTime day)
        {
            switch (day.DayOfWeek)
            {
                case DayOfWeek.Saturday:
                case DayOfWeek.Sunday:
                    return true;
                default:
                    {
                        string cacheKey = "holidays_" + day.Year;
                        IEnumerable<DateTime?> holidaysInYear;
                        if (_cache.TryGetValue(cacheKey, out holidaysInYear))
                            return holidaysInYear.Contains(day);
                        holidaysInYear = Holidays.Select(x => x.GetInstance(day.Year)).ToList();
                        _cache.Set(cacheKey, holidaysInYear);
                        return holidaysInYear.Contains(day);
                    }
            }

        }

        private class HolidayData
        {
            public DateTime HolidayDate { get; set; }
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
            {
                currentTime = currentTime.AddDays(1);
                currentTime = new DateTime(currentTime.Year, currentTime.Month, currentTime.Day).Add(StartBusinessHoursTime);
            }

            DateTime eta = currentTime.AddTicks(ticksToAdd);
            var amountPastBusinessDay = eta.Subtract(currentTime.Date.Add(EndBusinessHoursTime));
            while (amountPastBusinessDay.Ticks > 0)
            {
                var nextWorkingDay = currentTime.AddWorkingDays(1, _workingDayCultureInfo).Date;

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
            currentDate = currentDate.AddWorkingDays(days, _workingDayCultureInfo);

            return currentDate.Add(timeComponent);
        }

        private Task<DateTime> SubtractBusinessDays(DateTime startTime, int v)
        {
            throw new NotSupportedException("Currently subtracting business days is not supported.");
        }


        /// <summary>
        /// <a href="http://en.wikipedia.org/wiki/Victoria_Day">Victoria Day</a> occurs on the last Monday before May 25<sup>th</sup> of the year specified.
        /// </summary>
        private class VictoriaDay : NthDayOfWeekInMonthHoliday
        {
            public VictoriaDay() : base("Victoria Day", 1, DayOfWeek.Monday, 5, CountDirection.FromLast) { }

            public override DateTime? GetInstance(int year)
            {
                // fix for when the base class returns a date >= 25
                var theDate = base.GetInstance(year);
                if (theDate.HasValue && theDate.Value.Day >= 25)
                    theDate = theDate.Value.AddDays(-7);
                return theDate;
            }
        }


        private class NonWeekendHoliday : Holiday
        {
            public NonWeekendHoliday(Holiday holiday) : base(holiday.Name)
            {
                _holiday = holiday;
            }

            private readonly Holiday _holiday;

            public override DateTime? GetInstance(int year)
            {
                return EnsureNonWeekend(_holiday.GetInstance(year));
            }

            private static DateTime? EnsureNonWeekend(DateTime? date)
            {
                if (date.HasValue)
                {
                    if (date.Value.DayOfWeek == DayOfWeek.Saturday)
                    {
                        return date.Value.AddDays(2);
                    }
                    else if (date.Value.DayOfWeek == DayOfWeek.Sunday)
                    {
                        return date.Value.AddDays(1);
                    }
                }
                return date;
            }

            public override bool IsInstanceOf(DateTime date)
            {
                var holidayDate = _holiday.GetInstance(date.Year);
                return holidayDate != null && holidayDate.Value.Date == date.Date;
            }
        }

    }
}

using System;
using System.Globalization;

namespace CoE.Ideas.Shared.Extensions
{
    public static class DateTimeExtensions
    {
        private static GregorianCalendar _gc = new GregorianCalendar();

        public static long ToUnixTimestamp(this DateTime dateTime)
        {
            DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            if (dateTime.Kind == DateTimeKind.Local)
                return Convert.ToInt64(dateTime.ToUniversalTime().Subtract(epoch).TotalSeconds);
            else
                return Convert.ToInt64(dateTime.Subtract(epoch).TotalSeconds);
        }

        /// <summary>
        /// Returns string like "Tomorrow at 11:37 AM", or "27 minutes ago"
        /// </summary>
        /// <param name="dateTime"></param>
        public static string ToStringRelativeToNow(this DateTime dateTime, DateTime relativeToNow, string timeFormat = "h:mm tt")
        {
            if (dateTime.Subtract(relativeToNow).Ticks > 0)
                return ToStringRelativeToNowFutureDate(dateTime, relativeToNow, timeFormat);
            else
				return ToStringRelativeToNowPastDate(dateTime, relativeToNow);
		}

        private static string ToStringRelativeToNowFutureDate(DateTime dtEvent, DateTime relativeToNow, string timeFormat)
        {
			// special cases for "today" and "tomorrow"
			DateTime eventDay = dtEvent.Date;
            DateTime nowDay = relativeToNow.Date;
            if (nowDay == eventDay)
                return $"{dtEvent.ToString(timeFormat)} today";
            else if (nowDay.AddDays(1) == eventDay)
                return $"{dtEvent.ToString(timeFormat)} tomorrow";
            else if (nowDay.GetWeekOfMonth() == dtEvent.GetWeekOfMonth())
            {
                return dtEvent.DayOfWeek.ToString() + " at " + dtEvent.ToString(timeFormat);
            }
            else if (nowDay.Year == dtEvent.Year)
            {
                return $"{dtEvent.ToString(timeFormat)} on {dtEvent.ToString("dddd, MMMM dd")}";
            }
            else
            {
                return $"{dtEvent.ToString(timeFormat)} on {dtEvent.ToLongDateString()}";
            }
        }

        private static string ToStringRelativeToNowPastDate(DateTime dtEvent, DateTime now)
        {
            // shamelsessly stolen from https://stackoverflow.com/questions/15844451/string-extensions-for-today-yesterday-8-seconds-ago-etc-in-c-sharp
            TimeSpan TS = DateTime.Now - dtEvent;
            int intYears = now.Year - dtEvent.Year;
            int intMonths = now.Month - dtEvent.Month;
            int intDays = TS.Days;
            int intHours = TS.Hours;
            int intMinutes = TS.Minutes;
            int intSeconds = TS.Seconds;
            if (intYears > 0 && intDays > 365) return String.Format("{0} {1} ago", intYears, (intYears == 1) ? "year" : "years");
            else if (intMonths > 0 && intDays > 30) return String.Format("{0} {1} ago", intMonths, (intMonths == 1) ? "month" : "months");
            else if (intDays > 0) return String.Format("{0} {1} ago", intDays, (intDays == 1) ? "day" : "days");
            else if (intHours > 0) return String.Format("{0} {1} ago", intHours, (intHours == 1) ? "hour" : "hours");
            else if (intMinutes > 0) return String.Format("{0} {1} ago", intMinutes, (intMinutes == 1) ? "minute" : "minutes");
            else if (intSeconds > 0) return String.Format("{0} {1} ago", intSeconds, (intSeconds == 1) ? "second" : "seconds");
            else
            {
                return String.Format("{0} {1} ago", dtEvent.ToShortDateString(), dtEvent.ToShortTimeString());
            }
        }

        public static int GetWeekOfMonth(this DateTime time)
        {
            // from https://stackoverflow.com/questions/2136487/calculate-week-of-month-in-net/2136549#2136549
            DateTime first = new DateTime(time.Year, time.Month, 1);
            return time.GetWeekOfYear() - first.GetWeekOfYear() + 1;
        }

        static int GetWeekOfYear(this DateTime time)
        {
            return _gc.GetWeekOfYear(time, CalendarWeekRule.FirstDay, DayOfWeek.Sunday);
        }




    }
}

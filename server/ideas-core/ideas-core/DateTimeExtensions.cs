using System;
using System.Collections.Generic;
using System.Text;

namespace CoE.Ideas.Core
{
    public static class DateTimeExtensions
    {
        public static long ToUnixTimestamp(this DateTime dateTime)
        {
            DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            if (dateTime.Kind == DateTimeKind.Local)
                return Convert.ToInt64(dateTime.ToUniversalTime().Subtract(epoch).TotalSeconds);
            else
                return Convert.ToInt64(dateTime.Subtract(epoch).TotalSeconds);
        }
    }
}

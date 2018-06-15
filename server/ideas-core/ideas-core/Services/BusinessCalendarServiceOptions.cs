using System;
using System.Collections.Generic;
using System.Text;

namespace CoE.Ideas.Core.Services
{
    internal class BusinessCalendarServiceOptions
    {
        public TimeSpan StartBusinessHoursTime { get; set; } = new TimeSpan(7, 0, 0);
        public TimeSpan EndBusinessHoursTime { get; set; } = new TimeSpan(17, 0, 0);
    }
}

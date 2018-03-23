using CoE.Ideas.Core.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CoE.Ideas.Core.Services
{
    internal interface IBusinessCalendarService
    {
        /// <summary>
        /// Add the amount of time specified to the startTime, 
        /// excluding Busines Hours and holidays
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        Task<DateTime> AddBusinessTime(DateTime startTime, TimeSpan time);

        /// <summary>
        /// Adds the number of business days to startTime, excluding holidays
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="days"></param>
        /// <returns></returns>
        Task<DateTime> AddBusinessDays(DateTime startTime, int days);
    }
}

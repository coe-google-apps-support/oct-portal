using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CoE.Ideas.Shared
{
    public interface IHealthCheckable
    {
        /// <summary>
        /// Performs a health check on this object or service.
        /// </summary>
        /// <returns>
        /// Returns an object suitable for output when determining
        /// if this object or service is in a healthy state.
        /// </returns>
        /// <remarks>
        /// This is used for instance in repositories to determine if 
        /// the connection to its uderlying database is working, and 
        /// other diagnostic information.
        /// The ideas-server calls all DI registered services and 
        /// reports back the status of the services at /api/healthcheck
        /// </remarks>
        Task<IDictionary<string, object>> HealthCheckAsync();
    }
}

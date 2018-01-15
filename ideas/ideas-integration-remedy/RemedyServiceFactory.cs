using RemedyService;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoE.Ideas.Remedy
{
    public static class RemedyServiceFactory
    {
        /// <summary>
        /// Creates a default Remedy Service object using configuration 
        /// from defauklt configuration file
        /// </summary>
        /// <returns></returns>
        public static IRemedyService CreateRemedyService()
        {
            var client = new New_Port_0PortTypeClient();
            return new RemedyServiceImpl(client);
        }
    }
}

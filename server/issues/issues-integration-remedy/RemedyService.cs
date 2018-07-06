using System;
using CoE.Issues.Core.Data;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using CoE.Issues.Remedy.Watcher.RemedyServiceReference;

namespace CoE.Issues.Remedy
{
    public class RemedyService : IRemedyService
    {
        public RemedyService(New_Port_0PortType remedyClient,
            IOptions<RemedyServiceOptions> options,
            Serilog.ILogger logger)
        {
            _remedyClient = remedyClient ?? throw new ArgumentNullException("remedyClient");
            if (options == null)
                throw new ArgumentNullException("options");
            _options = options?.Value;
            _logger = logger ?? throw new ArgumentNullException("logger");
        }

        private readonly New_Port_0PortType _remedyClient;
        private readonly RemedyServiceOptions _options;
        private readonly Serilog.ILogger _logger;


       
    }
}

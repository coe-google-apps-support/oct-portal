using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using CoE.Ideas.Remedy.Watcher.RemedyServiceReference;
using Microsoft.Extensions.Options;

namespace CoE.Ideas.Remedy.Watcher
{
    public class RemedyService : IRemedyService
    {
        public RemedyService(New_Port_0PortType remedyClient,
            Serilog.ILogger logger,
            IOptions<RemedyCheckerOptions> options)
        {
            _remedyClient = remedyClient ?? throw new ArgumentNullException("remedyClient");
            _logger = logger ?? throw new ArgumentException("logger");
            if (options == null || options.Value == null)
                throw new ArgumentNullException("options");
            _options = options.Value;

            _logger.Information("Created Remedy Service, endpoint should be {Url}", options.Value.ApiUrl);
        }

        private New_Port_0PortType _remedyClient;
        private Serilog.ILogger _logger;
        private RemedyCheckerOptions _options;

        public async Task<IEnumerable<OutputMapping1GetListValues>> GetRemedyChangedWorkItems(DateTime fromUtc)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();

            try
            {
                var authInfo = new AuthenticationInfo()
                {
                    userName = _options.ServiceUserName,
                    password = _options.ServicePassword,
                    authentication = "?",
                    locale = "?",
                    timeZone = "?"
                };

                var remedyResponse = await _remedyClient.New_Get_Operation_0Async(
                    new New_Get_Operation_0Request(
                        authInfo,
                        _options.TemplateName,
                        fromUtc.ToString("O"))); // TODO: apply time component - like format "O" or "yyyy-MM-dd"
                int count = 0;
                if (remedyResponse != null && remedyResponse.getListValues != null)
                    count = remedyResponse.getListValues.Length;
                _logger.Information($"Remedy returned { count } changed work item records in { watch.Elapsed.TotalMilliseconds }ms");

                return remedyResponse.getListValues;
            }
            catch (Exception err)
            {
                _logger.Error(err, $"Unable to get response from Remedy: { err.Message }");
                throw;
            }
        }
    }
}

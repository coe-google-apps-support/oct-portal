using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RemedyServiceReference;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoE.Ideas.Remedy.Watcher
{
    public class RemedyChecker
    {
        public RemedyChecker(New_Port_0PortType remedyClient,
            ITopicClient topiClient,
            ILogger<RemedyChecker> logger,
            IOptions<RemedyCheckerOptions> options)
        {
            _remedyClient = remedyClient ?? throw new ArgumentNullException("remedyClient");
            _topicClient = topiClient ?? throw new ArgumentNullException("topiClient");
            _logger = logger ?? throw new ArgumentException("logger");

            if (options == null || options.Value == null)
                throw new ArgumentNullException("options");
            _options = options.Value;

            TryReadLastPollTime();
        }


        private readonly New_Port_0PortType _remedyClient;
        private readonly ITopicClient _topicClient;
        private readonly ILogger<RemedyChecker> _logger;
        private readonly RemedyCheckerOptions _options;

        private const string ResultFilePrefix = "RemedyCheckerLog";

        private DateTime lastPollTimeUtc;
        private void TryReadLastPollTime()
        {
            bool success = false;
            if (Directory.Exists(_options.TempDirectory))
            {
                // get the latest file starting with "RemedyCheckerLog"
                var latest = new DirectoryInfo(_options.TempDirectory)
                    .GetFiles("RemedyCheckerLog*", SearchOption.TopDirectoryOnly)
                    .OrderByDescending(x => x.LastWriteTimeUtc)
                    .FirstOrDefault();
                if (latest != null)
                {
                    try
                    {
                        using (StreamReader file = File.OpenText(latest.FullName))
                        {
                            var lastPollResult = (RemedyPollResult)(new JsonSerializer()
                                .Deserialize(file, typeof(RemedyPollResult)));
                            lastPollTimeUtc = lastPollResult.EndTimeUtc;
                            success = true;
                        }
                    }
                    catch (Exception err)
                    {
                        // TODO: keep going through files until we find a good one?
                        _logger.LogCritical($"Unable to get last time we polled remedy for work item changes: { err.Message }");
                    }
                }
            }
            if (!success)
                lastPollTimeUtc = new DateTime(2017, 1, 1); //DateTime.Now.AddDays(-3);
        }

        public async Task<RemedyPollResult> Poll()
        {
            Stopwatch watch = null;
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                watch = new Stopwatch();
                _logger.LogDebug($"Polling Remedy using start time of { lastPollTimeUtc } (UTC) ");
                watch.Start();
            }
            else
                _logger.LogInformation("Polling Remedy");

            var result = new RemedyPollResult(lastPollTimeUtc);
            IEnumerable<OutputMapping1GetListValues> workItemsChanged = null;
            try { workItemsChanged = await TryGetRemedyChangedWorkItems(); }
            catch (Exception err)
            {
                result.ProcessErrors.Add(new ProcessError() { ErrorMessage = err.Message });
            }
            if (workItemsChanged != null && workItemsChanged.Any())
            {
                await ProcessWorkItemsChanged(workItemsChanged, result);
            }
            result.EndTimeUtc = result.RecordsProcesed.Any()
                ? result.RecordsProcesed.Max(x => x.Last_Modified_Date)
                : lastPollTimeUtc;

            SaveResult(result);

            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug($"Finished Polling Remedy in { watch.Elapsed.TotalSeconds}s");
            }
            else
                _logger.LogInformation("Finished Polling Remedy");

            return result;
        }

        private void SaveResult(RemedyPollResult result)
        {
            var filename = Path.Combine(_options.TempDirectory, $"{ResultFilePrefix}_{DateTime.Now.ToString("yyyyMMdd_HHmmss")}.log");
            using (StreamWriter file = File.CreateText(filename))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, result);
            }
        }

        private async Task<IEnumerable<OutputMapping1GetListValues>> TryGetRemedyChangedWorkItems()
        {
            Stopwatch watch = null;
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                watch = new Stopwatch();
                watch.Start();
            }
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
                        lastPollTimeUtc.ToString("O"))); // TODO: apply time component - like format "O" or "yyyy-MM-dd"
                if (_logger.IsEnabled(LogLevel.Debug))
                {
                    int count = 0;
                    if (remedyResponse != null && remedyResponse.getListValues != null)
                        count = remedyResponse.getListValues.Length;
                    _logger.LogDebug($"Remedy returned { count } changed work item records in { watch.Elapsed.TotalMilliseconds }ms");
                }

                return remedyResponse.getListValues;
            }
            catch (Exception err)
            {
                _logger.LogCritical(err, $"Unable to get response from Remedy: { err.Message }");
                throw;
            }
            finally
            {
                if (_logger.IsEnabled(LogLevel.Debug))
                    watch.Stop();
            }
        }

        private async Task ProcessWorkItemsChanged(IEnumerable<OutputMapping1GetListValues> workItemsChanged,
            RemedyPollResult result)
        {
            Stopwatch watch = null;
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                watch = new Stopwatch();
                watch.Start();
            }

            try
            {
                int count = 0;
                foreach (var workItem in workItemsChanged)
                {
                    var error = await TryProcessWorkItemChanged(workItem);

                    if (error == null)
                    {
                        result.RecordsProcesed.Add(workItem);
                    }
                    else
                    {
                        result.ProcessErrors.Add(new ProcessError()
                        {
                            WorkItem = workItem,
                            ErrorMessage = error.Message
                        });
                    }
                    count++;
                }
                if (_logger.IsEnabled(LogLevel.Debug))
                {
                    TimeSpan avg = count > 0 ? watch.Elapsed / count : TimeSpan.Zero;
                    _logger.LogDebug($"Processed { count } work item changes in { watch.Elapsed }. Average = { avg.TotalMilliseconds }ms/record ");
                }
            }
            finally
            {
                if (_logger.IsEnabled(LogLevel.Debug))
                {
                    watch.Stop();
                }
            }

        }

        private async Task<Exception> TryProcessWorkItemChanged(
            OutputMapping1GetListValues workItem)
        {
            try
            {
                var message = new Message
                {
                    Label = "Remedy Work Item Changed"
                };
                message.UserProperties.Add("Event", "StatusUpdated");
                message.UserProperties.Add("WorkItemId", workItem.WorkOrderID);
                message.UserProperties.Add("WorkItemStatus", workItem.Status.ToString());

                string value =  JsonConvert.SerializeObject(workItem);
                message.Body = Encoding.UTF8.GetBytes(value);

                await _topicClient.SendAsync(message);
                return null;
            }
            catch (Exception e)
            {
                Guid correlationId = Guid.NewGuid();
                _logger.LogError(e, $"Unable to process work item changed (correlationId {correlationId}): {e.Message}");
                if (_logger.IsEnabled(LogLevel.Debug))
                {
                    _logger.LogDebug($"Work item change that caused processing error (correlationId {correlationId}): { workItem }");
                }

                return e;
            }
    }
    }
}

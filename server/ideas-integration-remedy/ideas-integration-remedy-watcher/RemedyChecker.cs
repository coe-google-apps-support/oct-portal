using CoE.Ideas.Core.ServiceBus;
using CoE.Ideas.Remedy.Watcher.RemedyServiceReference;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoE.Ideas.Remedy.Watcher
{
    public class RemedyChecker : IRemedyChecker
    {
        public RemedyChecker(IRemedyService remedyService,
            IInitiativeMessageSender initiativeMessageSender,
            Serilog.ILogger logger,
            IOptions<RemedyCheckerOptions> options)
        {
            _remedyService = remedyService ?? throw new ArgumentNullException("remedyService");
            _initiativeMessageSender = initiativeMessageSender ?? throw new ArgumentNullException("initiativeMessageSender");
            _logger = logger ?? throw new ArgumentException("logger");

            if (options == null || options.Value == null)
                throw new ArgumentNullException("options");
            _options = options.Value;


            TryReadLastPollTime();
        }


        private readonly IRemedyService _remedyService;
        private readonly IInitiativeMessageSender _initiativeMessageSender;
        private readonly Serilog.ILogger _logger;
        private RemedyCheckerOptions _options;

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
                        _logger.Error($"Unable to get last time we polled remedy for work item changes: { err.Message }");
                    }
                }
            }
            if (!success)
                lastPollTimeUtc = new DateTime(2017, 1, 1); //DateTime.Now.AddDays(-3);
        }

        public async Task<RemedyPollResult> Poll()
        {
            var result = await PollAsync(lastPollTimeUtc);
            SaveResult(result);
            return result;
        }


        public async Task<RemedyPollResult> PollAsync(DateTime fromUtc)
        {
            Stopwatch watch = new Stopwatch();

            var result = new RemedyPollResult(fromUtc);
            IEnumerable<OutputMapping1GetListValues> workItemsChanged = null;
            try { workItemsChanged = await _remedyService.GetRemedyChangedWorkItems(fromUtc); }
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

            _logger.Information($"Finished Polling Remedy in { watch.Elapsed.TotalSeconds}s");

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



        private async Task ProcessWorkItemsChanged(IEnumerable<OutputMapping1GetListValues> workItemsChanged,
            RemedyPollResult result)
        {
            Stopwatch watch =  new Stopwatch();
            watch.Start();

            int count = 0;
            foreach (var workItem in workItemsChanged)
            {
                if (workItem.Last_Modified_Date <= lastPollTimeUtc)
                {
                    _logger.Warning($"WorkItem { workItem.Last_Modified_Date } has a last modified date less than or equal to our previous poll time, so ignoring");
                    continue;
                }

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
            TimeSpan avg = count > 0 ? watch.Elapsed / count : TimeSpan.Zero;
            _logger.Information($"Processed { count } work item changes in { watch.Elapsed }. Average = { avg.TotalMilliseconds }ms/record ");
        }

        private async Task<Exception> TryProcessWorkItemChanged(
            OutputMapping1GetListValues workItem)
        {
            try
            {
                await _initiativeMessageSender.SendWorkOrderUpdatedAsync(new WorkOrderUpdatedEventArgs()
                {
                    WorkOrderId = workItem.WorkOrderID,
                    UpdatedStatus = workItem.Status.ToString(),
                    UpdatedDateUtc = workItem.Last_Modified_Date
                });
                return null;
            }
            catch (Exception e)
            {
                Guid correlationId = Guid.NewGuid();
                _logger.Error(e, $"Unable to process work item changed (correlationId {correlationId}): {e.Message}");
                _logger.Debug($"Work item change that caused processing error (correlationId {correlationId}): { workItem }");

                return e;
            }
    }

    }
}

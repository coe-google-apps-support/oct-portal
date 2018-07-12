using AutoMapper;
using CoE.Ideas.Shared.People;
using CoE.Issues.Core.Data;
using CoE.Issues.Core.Remedy;
using CoE.Issues.Core.ServiceBus;
using CoE.Issues.Remedy.Watcher.RemedyServiceReference;
using CoE.Ideas.Shared.Extensions;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
namespace CoE.Issues.Remedy.Watcher
{
    public class RemedyChecker : IRemedyChecker
    {
        public RemedyChecker(
            IIssueMessageSender issueMessageSender,
            IRemedyChangedReceiver remedyChangedReceiver,
            IPeopleService peopleService,
            IMapper mapper,
            Serilog.ILogger logger,
            IOptions<RemedyCheckerOptions> options)
        {
            _issueMessageSender = issueMessageSender ?? throw new ArgumentNullException("issueMessageSender");
            _remedyChangedReceiver = remedyChangedReceiver ?? throw new ArgumentNullException("remedyChangedReceiver");
            _peopleService = peopleService ?? throw new ArgumentException("peopleService");
            _mapper = mapper ?? throw new ArgumentNullException("mapper");
            _logger = logger ?? throw new ArgumentException("logger");

            if (options == null || options.Value == null)
                throw new ArgumentNullException("options");
            _options = options.Value;
        }

        private readonly IIssueMessageSender _issueMessageSender;
        private readonly IRemedyChangedReceiver _remedyChangedReceiver;
        private readonly IPeopleService _peopleService;
        private readonly IMapper _mapper;
        private readonly Serilog.ILogger _logger;
        private RemedyCheckerOptions _options;

        private const string ResultFilePrefix = "RemedyCheckerLog";

        private DateTime lastPollTimeUtc;

        public DateTime TryReadLastPollTime()
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
                lastPollTimeUtc = new DateTime(2018, 7, 10); //DateTime.Now.AddDays(-3);

            return lastPollTimeUtc;
        }

        public RemedyPollResult Poll()
        {
            TryReadLastPollTime();
            _logger.Information("Using last poll time of {PollTime}", lastPollTimeUtc);

            var result = PollFromDate(lastPollTimeUtc);
            SaveResult(result);
            return result;
        }

        public RemedyPollResult PollFromDate(DateTime fromUtc)
        {
            Stopwatch watch = new Stopwatch();

            var result = new RemedyPollResult(fromUtc);

            _remedyChangedReceiver.ReceiveChanges(fromUtc, async(incident, token) =>
            {
                // send incident to service bus so the sblistener can deal with it
                await TryProcessIssue(incident);

                result.RecordsProcesed.Add(incident);
            });

            if (result.RecordsProcesed.Any())
            {
                long maxLastModifiedDate = result.RecordsProcesed.Max(x => x.LAST_MODIFIED_DATE);
                result.EndTimeUtc = maxLastModifiedDate.ToUnixTimestamp();
            }
            else
            {
                result.EndTimeUtc = lastPollTimeUtc;
            }

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

        /// <summary>
        /// Tries to process Remedy's output mapping so it fits into an IssueCreatedEventArgs object.
        /// TODO: we need to convert the Assignee 3+3 to an email so Octava can use it
        /// </summary>
        /// <param name="workItem">The generated workItem object. This is generated from a SOAP interface.</param>
        /// <returns>An async task that resolves with the IssueCreatedEventArgs.</returns>
        protected virtual async Task<IssueCreatedEventArgs> TryProcessIssue(Incident issue)
        {
            try
            {
                var args = new IssueCreatedEventArgs
                {
                    Incident = issue
                };

                //TODO: add the users' info

                if (!string.IsNullOrWhiteSpace(issue.SUBMITTER))
                    args.Submitter = await GetPersonData(issue.SUBMITTER);
                if (!string.IsNullOrWhiteSpace(issue.ASSIGNEE_LOGIN_ID))
                    args.Assignee = await GetPersonData(issue.ASSIGNEE_LOGIN_ID);
                if (!string.IsNullOrWhiteSpace(issue.CUSTOMER_LOGIN_ID))
                    args.Assignee = await GetPersonData(issue.CUSTOMER_LOGIN_ID);



                await _issueMessageSender.SendIssueCreatedAsync(args);
                return args;
            }
            catch (Exception e)
            {
                Guid correlationId = Guid.NewGuid();
                _logger.Error(e, $"Unable to process work item changed (correlationId {correlationId}): {e.Message}");
                _logger.Debug($"Work item change that caused processing error (correlationId {correlationId}): { issue }");
                throw;
            }
        }

        private async Task<PersonData> GetPersonData(string user3and3)
        {
            try
            {
                return await _peopleService.GetPersonAsync(user3and3);
            }
            catch (Exception err)
            {
                // log and eat the exception
                _logger.Error(err, "Unable to get person info for user {user3and3}", user3and3);
                return null;
            }
        }
    }
}

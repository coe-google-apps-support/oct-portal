using AutoMapper;
using CoE.Ideas.Shared.People;
using CoE.Issues.Core.Data;
using CoE.Issues.Core.ServiceBus;
using CoE.Issues.Remedy.Watcher.RemedyServiceReference;
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
        public RemedyChecker(IRemedyService remedyService,
            IIssueMessageSender issueMessageSender,
            IMapper mapper,
            Serilog.ILogger logger,
            IPeopleService peopleService,
            IOptions<RemedyCheckerOptions> options)
        {
            _remedyService = remedyService ?? throw new ArgumentNullException("remedyService");
            _issueMessageSender = issueMessageSender ?? throw new ArgumentNullException("issueMessageSender");
            _mapper = mapper ?? throw new ArgumentNullException("mapper");
            _logger = logger ?? throw new ArgumentException("logger");
            _peopleService = peopleService ?? throw new ArgumentNullException("peopleService");


            if (options == null || options.Value == null)
                throw new ArgumentNullException("options");
            _options = options.Value;
        }

        private readonly IRemedyService _remedyService;
        private readonly IIssueMessageSender _issueMessageSender;
        private readonly IMapper _mapper;
        private readonly IPeopleService _peopleService;

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

        public async Task<RemedyPollResult> Poll()
        {
            TryReadLastPollTime();
            _logger.Information("Using last poll time of {PollTime}", lastPollTimeUtc);

            var result = await PollAsync(lastPollTimeUtc);
            SaveResult(result);
            return result;
        }

        public async Task<RemedyPollResult> PollAsync(DateTime fromUtc)
        {
            {
                Stopwatch watch = new Stopwatch();

                var result = new RemedyPollResult(fromUtc);
                IEnumerable<OutputMapping1GetListValues> workItemsChanged = null;
                try
                {
                    workItemsChanged = _remedyService.GetRemedyChangedWorkItems(fromUtc);
                }
                catch (Exception err)
                {
                    result.ProcessErrors.Add(new ProcessError() { ErrorMessage = err.Message });
                }
                if (workItemsChanged != null && workItemsChanged.Any())
                {
                    await ProcessWorkItemsChanged(workItemsChanged, result, fromUtc);
                }
                result.EndTimeUtc = result.RecordsProcesed.Any()
                    ? result.RecordsProcesed.Max(x => x.Last_Modified_Date)
                    : lastPollTimeUtc;

                _logger.Information($"Finished Polling Remedy in { watch.Elapsed.TotalSeconds}s");

                return result;
            }

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
            RemedyPollResult result, DateTime timestampUtc)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();

            int count = 0;
            foreach (var workItem in workItemsChanged)
            {
                _logger.Information("Processing workItemChanged for id {InstanceId}", workItem.InstanceId);

                // We can do the next line because this service will always be in the same time zone as Remedy
                DateTime lastModifiedDateUtc = workItem.Last_Modified_Date.ToUniversalTime();

                if (lastModifiedDateUtc <= timestampUtc)
                {
                    _logger.Warning($"WorkItem { workItem.Web_Incident_ID} has a last modified date less than or equal to our cutoff time, so ignoring ({ lastModifiedDateUtc } <= { timestampUtc }");
                    continue;
                }

                Exception error = null;
                try
                {
                    await TryProcessIssue(workItem);
                }
                catch (Exception err)
                {
                    error = err;
                }

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

        /// <summary>
        /// Tries to process Remedy's output mapping so it fits into an IssueCreatedEventArgs object.
        /// TODO: we need to convert the Assignee 3+3 to an email so Octava can use it
        /// </summary>
        /// <param name="workItem">The generated workItem object. This is generated from a SOAP interface.</param>
        /// <returns>An async task that resolves with the IssueCreatedEventArgs.</returns>
        protected virtual async Task<IssueCreatedEventArgs> TryProcessIssue(OutputMapping1GetListValues workItem)
        {

            string assignee3and3 = workItem.Assignee_Login_ID;
            string submitter3and3 = workItem.Submitter;

            PersonData assignee = null;
            PersonData submitter = null;

            if (string.IsNullOrWhiteSpace(assignee3and3))
            {
                _logger.Information("Assignee is empty");
            }
            else
            {
                _logger.Information("Looking up assignee with 3+3 {User3and3}", assignee3and3);
                try { assignee = await _peopleService.GetPersonAsync(assignee3and3); }
                catch (Exception err) {
                    assignee = null;
                    _logger.Warning(err, "Unable to get email for Remedy incident Assignee {User3and3}: {ErrorMessage}", assignee3and3, err.Message); }
            }


            if (string.IsNullOrWhiteSpace(submitter3and3))
            {
                _logger.Information("Submitter is empty");
            }
            else
            {
                _logger.Information("Looking up submitter with 3+3 {User3and3}", submitter3and3);
                try { submitter = await _peopleService.GetPersonAsync(submitter3and3); }
                catch (Exception err) {
                    submitter = null;
                    _logger.Warning(err, "Unable to get email for Remedy incident Submitter {User3and3}: {ErrorMessage}", submitter3and3, err.Message); }
            }

            IssueStatus? newIssueStatus = GetIssueStatusForRemedyStatus(workItem.Status);
            if (newIssueStatus == null)
            {
                _logger.Information("Abondining updated work item because an appropriate IssueStatus could not be determined from the Remedy Status {WorkItemStatus}", workItem.Status);
                return null;
            }

            try
            {
                // convert Remedy object to IssueCreatedEventArgs
                var args = _mapper.Map<OutputMapping1GetListValues, IssueCreatedEventArgs>(workItem);
                if (assignee != null)
                {
                    args.AssigneeEmail = assignee.Email;
                }
                if (submitter != null)
                {
                    args.RequestorEmail = submitter.Email;
                    args.RequestorGivenName = submitter.GivenName;
                    args.RequestorSurnName = submitter.Surname;
                    args.RequestorTelephone = submitter.Telephone;
                    args.RequestorDisplayName = submitter.DisplayName;
                }
                await _issueMessageSender.SendIssueCreatedAsync(args);
                return args;
            }
            catch (Exception e)
            {
                Guid correlationId = Guid.NewGuid();
                _logger.Error(e, $"Unable to process work item changed (correlationId {correlationId}): {e.Message}");
                _logger.Debug($"Work item change that caused processing error (correlationId {correlationId}): { workItem }");
                throw;
            }
        }

        protected virtual IssueStatus? GetIssueStatusForRemedyStatus(StatusType remedyStatusType)
        {
            // here we have the business logic of translating Remedy statuses into our statuses
            IssueStatus newIdeaStatus;
            switch (remedyStatusType)
            {
                case StatusType.Assigned:
                    newIdeaStatus = IssueStatus.Submit;
                    break;
                case StatusType.Cancelled:
                    newIdeaStatus = IssueStatus.Cancelled;
                    break;
                case StatusType.Pending:
                    newIdeaStatus = IssueStatus.Review;
                    break;
                case StatusType.InProgress:
                    newIdeaStatus = IssueStatus.Collaborate;
                    break;
                case StatusType.Resolved:
                    newIdeaStatus = IssueStatus.Deliver;
                    break;
                case StatusType.Closed:
       
                default:
                    return null; // no change
            }
            return newIdeaStatus;
        }
    }
}

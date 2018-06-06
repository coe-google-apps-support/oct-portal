using CoE.Ideas.Core.Data;
using CoE.Ideas.Core.ServiceBus;
using CoE.Ideas.Shared.Extensions;
using CoE.Ideas.Shared.Security;
using CoE.Ideas.Shared.WordPress;
using EnsureThat;
using Serilog.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CoE.Ideas.Webhooks
{
    class WebhookPoster
    {
        public WebhookPoster(
           IInitiativeMessageReceiver initiativeMessageReceiver,
           IWebhookUrlService webhookUrlService,
           IWordPressRepository wordPressRepository,
           Serilog.ILogger logger)
        {
            _initiativeMessageReceiver = initiativeMessageReceiver ?? throw new ArgumentNullException("initiativeMessageReceiver");
            _logger = logger ?? throw new ArgumentNullException("logger");
            _webhookUrlService = webhookUrlService ?? throw new ArgumentNullException("webhookUrlService");
            _wordPressRepository = wordPressRepository ?? throw new ArgumentNullException("wordPressRepository");

            _initiativeMessageReceiver.ReceiveMessages(initiativeCreatedHandler: OnNewInitiative, 
                statusChangedHandler: OnStatusChanged);
        }

        private async Task FireWebHooksAsync(Initiative initiative, WebhookEvents eventType, 
            Action<IDictionary<string, string>> data = null, 
            CancellationToken cancellationToken = default(CancellationToken))
        {
            using (LogContext.PushProperty("InitiativeId", initiative.Id))
            {
                var hooks = _webhookUrlService.GetWebhookUrls(eventType).ToList();
                _logger.Information("Webhooks will post to {Count} webhooks for event {EventType} for initiative {InitiativeId}", hooks.Count, eventType, initiative.Id);

                if (!hooks.Any())
                    return;
                var owner = await GetInitiativeOwner(initiative);
                var createDateAlberta = TimeZoneInfo.ConvertTimeFromUtc(initiative.CreatedDate.DateTime, AlbertaTimeZone);
                var values = new Dictionary<string, string>
                {
                    { "ID", initiative.Id.ToString()},
                    { "Title", initiative.Title},
                    { "CreatedDate", createDateAlberta.ToString()},
                    { "Description", initiative.Description},
                    { "OwnerName", owner?.Name},
                    { "OwnerEmail", owner?.Email}
                };

                data?.Invoke(values);
                var content = new FormUrlEncodedContent(values);

                using (var client = new HttpClient())
                {
                    foreach (var eventUrl in hooks)
                    {
                        try
                        {
                            var response = await client.PostAsync(eventUrl, content, cancellationToken);
                            var responseString = await response.Content.ReadAsStringAsync();
                            _logger.Information("Webhook ({EventType}) at url {Url} returned {StatusCode} with message {Message}", eventType, eventUrl, response.StatusCode, responseString);
                        }
                        catch (Exception err)
                        {
                            _logger.Error(err, "Error posting {EventType} to webhook with Url {Url}: {ErrorMessage}", eventType, eventUrl, err.Message);
                        }
                    }
                }
            }
        }

        private static TimeZoneInfo _albertaTimeZone;
        protected TimeZoneInfo AlbertaTimeZone
        {
            get
            {
                if (_albertaTimeZone == null)
                {
                    _albertaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("America/Edmonton");
                }
                return _albertaTimeZone;
            }
        }

        public async Task OnNewInitiative(InitiativeCreatedEventArgs initiativeCreatedEventArgs, CancellationToken cancellationToken)
        {
            EnsureArg.IsNotNull(initiativeCreatedEventArgs);
            EnsureArg.IsNotNull(initiativeCreatedEventArgs.Initiative);

            var owner = initiativeCreatedEventArgs.Owner;

            await FireWebHooksAsync(initiativeCreatedEventArgs.Initiative, 
                WebhookEvents.Created, 
 
                cancellationToken: cancellationToken);
        }

        private async Task <WordPressUser> GetInitiativeOwner(Initiative initiative)
        {
            try
            {
                return await _wordPressRepository.GetUserAsync(initiative.Stakeholders.Single(x => x.Type == StakeholderType.Requestor).PersonId);
            }
            catch(Exception err)
            {
                _logger.Error(err, "Unable to get the owner of initiative {InitiativeId}", initiative.Id);
                return null;
            }
        }

        public async Task OnStatusChanged(InitiativeStatusChangedEventArgs initiativeStatusDescriptionChangedEventArgs, CancellationToken cancellationToken)
        {
            EnsureArg.IsNotNull(initiativeStatusDescriptionChangedEventArgs);
            EnsureArg.IsNotNull(initiativeStatusDescriptionChangedEventArgs.Initiative);


            var initiative = initiativeStatusDescriptionChangedEventArgs.Initiative;
            _logger.Information("Initiative {InitiativeId} has had its status changed to {Status}, firing webhooks...", initiative.Id, initiative.Status.ToString());

                InitiativeStatusHistory lastStep = initiative.StatusHistories?.OrderByDescending(x => x.StatusEntryDateUtc).FirstOrDefault();
            DateTime expectedExitDateAlberta = lastStep == null || !lastStep.ExpectedExitDateUtc.HasValue
                ? DateTime.MinValue : TimeZoneInfo.ConvertTimeFromUtc(lastStep.ExpectedExitDateUtc.Value, AlbertaTimeZone);
            var nowDateAlberta = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, AlbertaTimeZone);


            string expectedCompletionDateString = expectedExitDateAlberta == DateTime.MinValue
                ? string.Empty
                : "by " + expectedExitDateAlberta.ToStringRelativeToNow(nowDateAlberta);

            await FireWebHooksAsync(initiative, 
                WebhookEvents.StatusChanged,
                data: values =>
                {
                    values["Status"] = initiative.Status.ToString();
                    values["ExpectedTime"] = expectedCompletionDateString;
                },
                cancellationToken: cancellationToken);
        }

        private readonly IInitiativeMessageReceiver _initiativeMessageReceiver;
        private readonly IWordPressRepository _wordPressRepository;
        private readonly IWebhookUrlService _webhookUrlService;
        private readonly Serilog.ILogger _logger;
    }
}

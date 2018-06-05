using CoE.Ideas.Core.Data;
using CoE.Ideas.Core.ServiceBus;
using CoE.Ideas.Shared.Extensions;
using CoE.Ideas.Shared.Security;
using EnsureThat;
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
           //IActiveDirectoryUserService activeDirectoryUserService,
           IInitiativeMessageSender initiativeMessageSender,
           IInitiativeMessageReceiver initiativeMessageReceiver,
           IWebhookUrlService webhookUrlService,
           Serilog.ILogger logger)
        {

            _initiativeMessageSender = initiativeMessageSender ?? throw new ArgumentNullException("initiativeMessageSender");
            _initiativeMessageReceiver = initiativeMessageReceiver ?? throw new ArgumentNullException("initiativeMessageReceiver");
            _logger = logger ?? throw new ArgumentNullException("logger");
            _webhookUrlService = webhookUrlService ?? throw new ArgumentNullException("webhook");

            _initiativeMessageReceiver.ReceiveMessages(initiativeCreatedHandler: OnNewInitiative,
                statusDescriptionChangedHandler: OnStatusDescriptionChanged);
        }
        public async Task OnNewInitiative(InitiativeCreatedEventArgs initiativeCreatedEventArgs, CancellationToken cancellationToken)
        {
            var initiative = initiativeCreatedEventArgs.Initiative;
            var owner = initiativeCreatedEventArgs.Owner;

            TimeZoneInfo albertaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("America/Edmonton");
            var createDateAlberta = TimeZoneInfo.ConvertTimeFromUtc(initiative.CreatedDate.DateTime, albertaTimeZone);
           
            foreach (var eventUrl in _webhookUrlService.GetWebhookUrls(WebhookEvents.Created))
            {
                using (var client = new HttpClient())
                {
                    var values = new Dictionary<string, string>
                    {
                        { "ID", initiative.Id.ToString()},
                        { "Title", initiative.Title},
                        { "OwnerName", owner.GetDisplayName()},
                        { "OwnerEmail", owner.GetEmail() },
                        { "CreatedDate", createDateAlberta.ToString()},
                        { "Description", initiative.Description}
                    };

                    var content = new FormUrlEncodedContent(values);

                    var response = await client.PostAsync(eventUrl, content);

                    var responseString = await response.Content.ReadAsStringAsync();

                }
            }
        }

        public async Task OnStatusDescriptionChanged(InitiativeStatusDescriptionChangedEventArgs initiativeStatusDescriptionChangedEventArgs, CancellationToken cancellationToken)
        {
            var initiative = initiativeStatusDescriptionChangedEventArgs.Initiative;
            var owner = initiativeStatusDescriptionChangedEventArgs.Owner;

            InitiativeStatusHistory lastStep = initiative.StatusHistories?.OrderByDescending(x => x.StatusEntryDateUtc).FirstOrDefault();
            TimeZoneInfo albertaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("America/Edmonton");
            DateTime createDateAlberta = TimeZoneInfo.ConvertTimeFromUtc(initiative.CreatedDate.DateTime, albertaTimeZone);
            var expectedExitDateAlberta = lastStep == null? DateTime.MinValue: TimeZoneInfo.ConvertTimeFromUtc(lastStep.ExpectedExitDateUtc.Value, albertaTimeZone);
            var nowDateAlberta = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, albertaTimeZone);
            string expectedCompletionDateString = expectedExitDateAlberta.ToStringRelativeToNow(nowDateAlberta);
            

            foreach (var eventUrl in _webhookUrlService.GetWebhookUrls(WebhookEvents.StatusChanged))
            {
                using (var client = new HttpClient())
                {
                    var values = new Dictionary<string, string>
                    {
                        { "ID", initiative.Id.ToString()},
                        { "Title", initiative.Title},
                        { "OwnerName", owner.GetDisplayName()},
                        { "OwnerEmail", owner.GetEmail() },
                        { "CreatedDate", createDateAlberta.ToString()},
                        { "Description", initiative.Description},
                        { "ExpectedTime", expectedCompletionDateString},
                        { "Status", initiative.Status.ToString()}
                    };

                    var content = new FormUrlEncodedContent(values);

                    var response = await client.PostAsync(eventUrl, content);

                    var responseString = await response.Content.ReadAsStringAsync();

                }
            }
        }

        //private readonly IActiveDirectoryUserService _activeDirectoryUserService;
        private readonly IInitiativeMessageSender _initiativeMessageSender;
        private readonly IInitiativeMessageReceiver _initiativeMessageReceiver;
        private readonly IWebhookUrlService _webhookUrlService;
        private readonly Serilog.ILogger _logger;



    }
}

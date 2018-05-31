using CoE.Ideas.Core.ServiceBus;
using CoE.Ideas.Shared.Security;
using EnsureThat;
using System;
using System.Collections.Generic;
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
           Serilog.ILogger logger)
        {

            _initiativeMessageSender = initiativeMessageSender ?? throw new ArgumentNullException("initiativeMessageSender");
            _initiativeMessageReceiver = initiativeMessageReceiver ?? throw new ArgumentNullException("initiativeMessageReceiver");
            _logger = logger ?? throw new ArgumentNullException("logger");

            _initiativeMessageReceiver.ReceiveMessages(initiativeCreatedHandler: OnNewInitiative,
                statusDescriptionChangedHandler: OnStatusDescriptionChanged);
        }
        public async Task OnNewInitiative(InitiativeCreatedEventArgs initiativeCreatedEventArgs, CancellationToken cancellationToken)
        {
            var initiative = initiativeCreatedEventArgs.Initiative;
            var owner = initiativeCreatedEventArgs.Owner;

            TimeZoneInfo albertaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("America/Edmonton");
            var createDateAlberta = TimeZoneInfo.ConvertTimeFromUtc(initiative.CreatedDate.DateTime, albertaTimeZone);

            using (var client = new HttpClient())
            {
                var values = new Dictionary<string, string>
                {
                    { "Action", "Create" },
                    { "ID", initiative.Id.ToString()},
                    { "Title", initiative.Title},
                    { "OwnerName", owner.GetDisplayName()},
                    { "OwnerEmail", owner.GetEmail() },
                    { "CreatedDate", createDateAlberta.ToString()},
                    { "Description", initiative.Description}
                };

                var content = new FormUrlEncodedContent(values);

                var response = await client.PostAsync("https://hooks.zapier.com/hooks/catch/3360483/afz7k1/", content);

                var responseString = await response.Content.ReadAsStringAsync();

            }
        }

        public async Task OnStatusDescriptionChanged(InitiativeStatusDescriptionChangedEventArgs initiativeStatusDescriptionChangedEventArgs, CancellationToken cancellationToken)
        {
            var initiative = initiativeStatusDescriptionChangedEventArgs.Initiative;
            var owner = initiativeStatusDescriptionChangedEventArgs.Owner;

            TimeZoneInfo albertaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("America/Edmonton");
            var createDateAlberta = TimeZoneInfo.ConvertTimeFromUtc(initiative.CreatedDate.DateTime, albertaTimeZone);

            using (var client = new HttpClient())
            {
                var values = new Dictionary<string, string>
                {
                    { "Action", "Status Description Changed" },
                    { "ID", initiative.Id.ToString()},
                    { "Title", initiative.Title},
                    { "OwnerName", owner.GetDisplayName()},
                    { "OwnerEmail", owner.GetEmail() },
                    { "CreatedDate", createDateAlberta.ToString()},
                    { "Description", initiative.Description}
                };

                var content = new FormUrlEncodedContent(values);

                var response = await client.PostAsync("https://hooks.zapier.com/hooks/catch/3360483/afvmki/", content);

                var responseString = await response.Content.ReadAsStringAsync();

            }
        }

        //private readonly IActiveDirectoryUserService _activeDirectoryUserService;
        private readonly IInitiativeMessageSender _initiativeMessageSender;
        private readonly IInitiativeMessageReceiver _initiativeMessageReceiver;
        private readonly Serilog.ILogger _logger;



    }
}

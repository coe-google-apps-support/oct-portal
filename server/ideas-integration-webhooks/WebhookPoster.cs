using CoE.Ideas.Core.ServiceBus;
using EnsureThat;
using System;
using System.Collections.Generic;
using System.Net.Http;
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

            using (var client = new HttpClient())
            {
                var values = new Dictionary<string, string>
                {
                    { "Action", "Create" },
                    { "ID", initiative.Id.ToString()},
                    { "Title", initiative.Title},
                    { "OwnerName", owner.ToString()},
                    { "OwnerEmail", owner.ToString() },
                    { "CreatedDate", initiative.CreatedDate.ToString()}
                };

                var content = new FormUrlEncodedContent(values);

                var response = await client.PostAsync("https://hooks.zapier.com/hooks/catch/3317083/a99f6h/", content);

                var responseString = await response.Content.ReadAsStringAsync();

            }

        }


        public async Task OnStatusDescriptionChanged(InitiativeStatusDescriptionChangedEventArgs initiativeStatusDescriptionChangedEventArgs, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        //private readonly IActiveDirectoryUserService _activeDirectoryUserService;
        private readonly IInitiativeMessageSender _initiativeMessageSender;
        private readonly IInitiativeMessageReceiver _initiativeMessageReceiver;
        private readonly Serilog.ILogger _logger;



    }
}

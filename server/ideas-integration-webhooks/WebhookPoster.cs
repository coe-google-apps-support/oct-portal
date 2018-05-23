using CoE.Ideas.Core.ServiceBus;
using System;
using System.Collections.Generic;
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
            throw new NotImplementedException();
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

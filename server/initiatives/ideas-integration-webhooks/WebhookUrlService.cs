using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoE.Ideas.Webhooks
{
    internal class WebhookUrlService: IWebhookUrlService
    {

        public WebhookUrlService(IOptions<WebhookUrlServiceOptions> options) { _options = options.Value; }

        private readonly WebhookUrlServiceOptions _options;

        public IEnumerable<string> GetWebhookUrls(WebhookEvents eventType)
        {
            switch (eventType)
            {
                case WebhookEvents.Created: return _options.CreatedUrl;
                case WebhookEvents.StatusChanged: return _options.StatusChangedUrl;
                default: throw new InvalidOperationException("Unkown webhook event");
            }
           
        }
      

    }
}

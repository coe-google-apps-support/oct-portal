using System;
using System.Collections.Generic;
using System.Text;

namespace CoE.Ideas.Webhooks
{
    public interface IWebhookUrlService
    {
        IEnumerable<string> GetWebhookUrls(WebhookEvents eventType);
    }
}

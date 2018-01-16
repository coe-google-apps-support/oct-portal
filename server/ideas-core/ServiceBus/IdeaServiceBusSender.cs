using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CoE.Ideas.Core.ServiceBus
{
    internal class IdeaServiceBusSender : IIdeaServiceBusSender
    {
        public IdeaServiceBusSender(ITopicSender<IdeaMessage> topicSender,
            IHttpContextAccessor httpContextAccessor)
        {
            _topicSenderr = topicSender ?? throw new ArgumentNullException("settings");
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException("httpContextAccessor");
        }
        private readonly ITopicSender<IdeaMessage> _topicSenderr;
        private readonly IHttpContextAccessor _httpContextAccessor;

        protected virtual void SetProperties(Idea idea, IDictionary<string, object> properties)
        {
            // get user info 
            var requestHeaders = _httpContextAccessor.HttpContext?.Request?.Headers;
            if (requestHeaders != null && requestHeaders.ContainsKey("Authorization"))
            {
                properties["AuthToken"] = requestHeaders["Authorization"].ToString();
            }
        }

        public async Task SendIdeaCreatedMessageAsync(Idea idea)
        {
            var message = new IdeaMessage() { IdeaId = idea.Id, Type = IdeaMessageType.IdeaCreated };
            var props = new Dictionary<string, object>();
            SetProperties(idea, props);

            await _topicSenderr.SendAsync(message, props);
        }

        public async Task SendIdeaUpdatedMessageAsync(Idea idea)
        {
            var message = new IdeaMessage() { IdeaId = idea.Id, Type = IdeaMessageType.IdeaUpdated };
            var props = new Dictionary<string, object>();
            SetProperties(idea, props);

            await _topicSenderr.SendAsync(message, props);
        }
    }
}

﻿using Microsoft.AspNetCore.Http;
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
            _httpContextAccessor = httpContextAccessor;
        }
        private readonly ITopicSender<IdeaMessage> _topicSenderr;
        private readonly IHttpContextAccessor _httpContextAccessor;

        protected virtual void SetProperties(Idea idea, IDictionary<string, object> properties)
        {
            // get user info 
            var requestHeaders = _httpContextAccessor?.HttpContext?.Request?.Headers;
            if (requestHeaders != null && requestHeaders.ContainsKey("Authorization"))
            {
                properties["AuthToken"] = requestHeaders["Authorization"].ToString();
            }
        }

        public async Task SendIdeaMessageAsync(Idea idea, IdeaMessageType messageType)
        {
            var message = new IdeaMessage() { IdeaId = idea.Id, Type = messageType };
            var props = new Dictionary<string, object>();
            SetProperties(idea, props);

            await _topicSenderr.SendAsync(message, props);
        }

        public async Task SendIdeaMessageAsync(Idea idea, IdeaMessageType messageType, Action<IDictionary<string, object>> onMessageHeaders)
        {
            var message = new IdeaMessage() { IdeaId = idea.Id, Type = messageType };
            var props = new Dictionary<string, object>();
            SetProperties(idea, props);

            if (onMessageHeaders != null)
                onMessageHeaders(props);

            await _topicSenderr.SendAsync(message, props);
        }
    }
}

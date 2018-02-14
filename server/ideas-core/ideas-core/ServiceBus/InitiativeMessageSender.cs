using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CoE.Ideas.Core.Security;
using CoE.Ideas.Core.WordPress;
using Microsoft.Azure.ServiceBus;

namespace CoE.Ideas.Core.ServiceBus
{
    internal class InitiativeMessageSender : IInitiativeMessageSender
    {
        public InitiativeMessageSender(ITopicClient topicClient, 
            IJwtTokenizer jwtTokenizer)
        {
            _topicClient = topicClient ?? throw new ArgumentNullException("topicClient");
            _jwtTokenizer = jwtTokenizer ?? throw new ArgumentNullException("jwtTokenizer");
        }

        private readonly ITopicClient _topicClient;
        private readonly IJwtTokenizer _jwtTokenizer;


        public Task SendInitiativeCreatedAsync(Idea initiative, ClaimsPrincipal ownerPrincipal)
        {
            var message = new Message
            {
                Label = "InitiativeCreated"
            };
            message.UserProperties["InitiativeId"] = initiative.Id;
            //message.UserProperties["OwnerClaims"] = ownerPrincipal.Claims;
            message.UserProperties["OwnerToken"] = _jwtTokenizer.CreateJwt(ownerPrincipal);

            return _topicClient.SendAsync(message);
        }

        public async Task SendInitiativeWorkItemCreatedAsync(Idea initiative, ClaimsPrincipal ownerPrincipal, string workOrderId)
        {
            var returnMessage = new Message
            {
                Label = "Remedy Work Item Created"
            };
            returnMessage.UserProperties["InitiativeId"] = initiative.Id;
            returnMessage.UserProperties["OwnerToken"] = _jwtTokenizer.CreateJwt(ownerPrincipal);
            returnMessage.UserProperties["WorkOrderId"] = workOrderId;
            await _topicClient.SendAsync(returnMessage);
        }
    }
}

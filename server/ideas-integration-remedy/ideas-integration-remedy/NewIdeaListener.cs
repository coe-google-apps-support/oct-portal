using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Text;
using System.Threading.Tasks;
using CoE.Ideas.Core;
using CoE.Ideas.Core.ServiceBus;
using CoE.Ideas.Core.WordPress;
using Microsoft.Azure.ServiceBus;

namespace CoE.Ideas.Remedy
{
    public class NewIdeaListener : IdeaListener
    {
        public NewIdeaListener(IIdeaRepository ideaRepository, 
            IWordPressClient wordPressClient,
            IRemedyService remedyService,
            //IActiveDirectoryUserService activeDirectoryUserService,
            ITopicClient topicClient)
            : base(ideaRepository, wordPressClient)
        {
            _remedyService = remedyService ?? throw new ArgumentNullException("remedyService");
            //_activeDirectoryUserService = activeDirectoryUserService ?? throw new ArgumentNullException("activeDirectoryUserService");
            _topicClient = topicClient ?? throw new ArgumentNullException("topicClient");
        }

        private readonly IRemedyService _remedyService;
        //private readonly IActiveDirectoryUserService _activeDirectoryUserService;
        private readonly ITopicClient _topicClient;

        protected override bool ShouldProcessMessage(IdeaMessage message)
        {
            return message.Type == IdeaMessageType.IdeaCreated;
        }

        protected override async Task ProcessIdeaMessage(IdeaMessage message, Idea idea, WordPressUser wordPressUser)
        {
            if (idea == null)
                throw new ArgumentNullException("idea");
            if (wordPressUser == null)
                throw new ArgumentNullException("wordPressUser");
            if (string.IsNullOrWhiteSpace(wordPressUser.Email))
                throw new ArgumentOutOfRangeException("wordpressUser email is empty");

            UserPrincipal adUser = null;
            //try
            //{
            //    adUser = _activeDirectoryUserService.GetADUser(wordPressUser.Email);
            //}
            //catch (Exception err)
            //{
            //    throw new InvalidOperationException($"Unable to find an Active Directory user with email { wordPressUser.Email }: { err.Message }");
            //}

            //if (adUser == null)
            //    throw new InvalidOperationException($"Unable to find an Active Directory user with email { wordPressUser.Email }");


            var remedyTicketId = await _remedyService.PostNewIdeaAsync(idea, wordPressUser, adUser?.SamAccountName);

            var returnMessage = new Message
            {
                Label = "Remedy Work Item Created"
            };
            returnMessage.UserProperties["IdeaId"] = idea.Id;
            returnMessage.UserProperties["WorkItemId"] = remedyTicketId;
            await _topicClient.SendAsync(returnMessage);
        }
    }
}

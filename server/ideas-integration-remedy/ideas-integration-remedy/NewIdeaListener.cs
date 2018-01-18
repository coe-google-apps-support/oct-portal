using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Text;
using System.Threading.Tasks;
using CoE.Ideas.Core;
using CoE.Ideas.Core.ServiceBus;
using CoE.Ideas.Core.WordPress;

namespace CoE.Ideas.Remedy
{
    public class NewIdeaListener : IdeaListener
    {
        public NewIdeaListener(IIdeaRepository ideaRepository, 
            IWordPressClient wordPressClient,
            IRemedyService remedyService,
            IActiveDirectoryUserService activeDirectoryUserService)
            : base(ideaRepository, wordPressClient)
        {
            _remedyService = remedyService;
            _activeDirectoryUserService = activeDirectoryUserService;
        }

        private readonly IRemedyService _remedyService;
        private readonly IActiveDirectoryUserService _activeDirectoryUserService;

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

            UserPrincipal adUser;
            try
            {
                adUser = _activeDirectoryUserService.GetADUser(wordPressUser.Email);
            }
            catch (Exception err)
            {
                throw new InvalidOperationException($"Unable to find an Active Directory user with email { wordPressUser.Email }: { err.Message }");
            }

            if (adUser == null)
                throw new InvalidOperationException($"Unable to find an Active Directory user with email { wordPressUser.Email }");


            await _remedyService.PostNewIdeaAsync(idea, wordPressUser, adUser.SamAccountName);
        }
    }
}

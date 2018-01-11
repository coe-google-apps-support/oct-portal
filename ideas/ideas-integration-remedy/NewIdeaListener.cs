using System;
using System.Collections.Generic;
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
            IRemedyService remedyService)
            : base(ideaRepository, wordPressClient)
        {
            _remedyService = remedyService;
        }

        private readonly IRemedyService _remedyService;

        protected override bool ShouldProcessMessage(IdeaMessage message)
        {
            return message.Type == IdeaMessageType.IdeaCreated;
        }

        protected override Task ProcessIdeaMessage(IdeaMessage message, Idea idea, string user3and3)
        {
            _remedyService.PostNewIdea(idea, user3and3);

            return Task.CompletedTask;
        }
    }
}

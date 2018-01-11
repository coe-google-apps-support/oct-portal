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
        public NewIdeaListener(IIdeaRepository ideaRepository, IWordPressClient wordPressClient) : base(ideaRepository, wordPressClient)
        {
        }

        protected override bool ShouldProcessMessage(IdeaMessage message)
        {
            return message.Type == IdeaMessageType.IdeaCreated;
        }

        protected override Task ProcessIdeaMessage(IdeaMessage message, Idea idea, WordPressUser user)
        {
            // TODO: add the idea to Remedy

            return Task.CompletedTask;
        }
    }
}

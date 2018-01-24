using CoE.Ideas.Core;
using CoE.Ideas.Core.ServiceBus;
using CoE.Ideas.Core.WordPress;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoE.Ideas.Remedy.SbListener
{
    public class RemedyItemUpdatedIdeaListener : IdeaListener
    {
        public RemedyItemUpdatedIdeaListener(
            IIdeaRepository ideaRepository, 
            IWordPressClient wordPressClient) 
            : base(ideaRepository, wordPressClient)
        {
        }
    }
}

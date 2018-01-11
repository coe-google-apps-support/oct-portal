using CoE.Ideas.Core;
using CoE.Ideas.Core.WordPress;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoE.Ideas.Remedy
{
    internal class RemedyService : IRemedyService
    {
        public RemedyService(/* initialization parameters go here */)
        {

        }

        public void PostNewIdea(Idea idea, WordPressUser user)
        {
            // TODO: Send idea to Remedy
            throw new NotImplementedException();
        }
    }
}

using CoE.Ideas.Core;
using CoE.Ideas.Core.WordPress;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoE.Ideas.Remedy
{
    public interface IRemedyService
    {
        void PostNewIdea(Idea idea, string user3and3);
    }
}

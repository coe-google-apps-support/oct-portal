using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CoE.Ideas.Remedy.Watcher
{
    public interface IRemedyChecker
    {
        Task<RemedyPollResult> PollAsync(DateTime fromUtc);
    }
}

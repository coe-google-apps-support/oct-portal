using System;
using System.Threading.Tasks;

namespace CoE.Issues.Remedy.Watcher
{
    public class RemedyChecker : IRemedyChecker
    {
        public Task<RemedyPollResult> PollAsync(DateTime fromUtc)
        {
            throw new NotImplementedException();
        }
    }
}

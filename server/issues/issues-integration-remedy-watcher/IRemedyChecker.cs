using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CoE.Issues.Remedy.Watcher
{
    public interface IRemedyChecker
    {
        RemedyPollResult PollFromDate(DateTime fromUtc);
        DateTime TryReadLastPollTime();
        RemedyPollResult Poll();
    }
}

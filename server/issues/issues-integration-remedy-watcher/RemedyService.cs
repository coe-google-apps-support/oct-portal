using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CoE.Issues.Remedy.Watcher.RemedyServiceReference;

namespace CoE.Issues.Remedy.Watcher
{
    class RemedyService : IRemedyService
    {
        public Task<IEnumerable<OutputMapping1GetListValues>> GetRemedyChangedWorkItems(DateTime fromUtc)
        {
            throw new NotImplementedException();
        }
    }
}

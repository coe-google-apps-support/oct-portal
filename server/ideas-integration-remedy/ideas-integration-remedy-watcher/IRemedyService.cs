using CoE.Ideas.Remedy.Watcher.RemedyServiceReference;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoE.Ideas.Remedy.Watcher
{
    public interface IRemedyService
    {
        Task<IEnumerable<OutputMapping1GetListValues>> GetRemedyChangedWorkItems(DateTime fromUtc);
    }
}
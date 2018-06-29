using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CoE.Issues.Remedy.Watcher.RemedyServiceReference;

namespace CoE.Issues.Remedy.Watcher
{
    interface IRemedyService
    {
        Task<IEnumerable<OutputMapping1GetListValues>> GetRemedyChangedWorkItems(DateTime fromUtc);
    }
}

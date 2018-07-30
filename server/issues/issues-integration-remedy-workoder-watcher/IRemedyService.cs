using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CoE.Issues.Remedy.WorkOrder.Watcher.RemedyServiceReference;

namespace CoE.Issues.Remedy.WorkOrder.Watcher
{
    public interface IRemedyService
    {
        IEnumerable<OutputMapping1GetListValues> GetRemedyChangedWorkItems(DateTime fromUtc);
    }
}

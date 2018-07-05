using CoE.Issues.Remedy.Watcher.RemedyServiceReference;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoE.Issues.Remedy.Watcher.Tests
{
    class MockRemedyService : IRemedyService
    {
        public Task<IEnumerable<OutputMapping1GetListValues>> GetRemedyChangedWorkItems(DateTime fromUtc)
        {
            throw new NotImplementedException();
        }
    }
}

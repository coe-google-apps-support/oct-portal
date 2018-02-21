using CoE.Ideas.Core;
using CoE.Ideas.Core.WordPress;
using CoE.Ideas.Remedy;
using CoE.Ideas.Remedy.Watcher.RemedyServiceReference;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoE.Ideas.EndToEnd.Tests
{
    internal class MockRemedyService : IRemedyService
    {
        public MockRemedyService()
        {
            WorkOrdersAdded = new List<RemedyWorkOrder>();
        }

        public ICollection<RemedyWorkOrder> WorkOrdersAdded { get; internal set; }


        public Task<string> PostNewIdeaAsync(Idea idea, string user3and3)
        {

            string workOrderID = Guid.NewGuid().ToString();
            WorkOrdersAdded.Add(new RemedyWorkOrder() { WorkOrderId = workOrderID, Idea = idea });

            return Task.FromResult(workOrderID);
        }

    }
}

using CoE.Ideas.Core.Data;
using CoE.Ideas.Remedy;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoE.Ideas.EndToEnd.Tests.Mocks
{
    internal class MockRemedyService : IRemedyService
    {
        public MockRemedyService()
        {
            WorkOrdersAdded = new List<RemedyWorkOrder>();
        }

        public ICollection<RemedyWorkOrder> WorkOrdersAdded { get; internal set; }


        public Task<string> PostNewIdeaAsync(Initiative idea, string user3and3)
        {

            string workOrderID = Guid.NewGuid().ToString();
            WorkOrdersAdded.Add(new RemedyWorkOrder() { WorkOrderId = workOrderID, Idea = idea });

            return Task.FromResult(workOrderID);
        }

    }
}

using CoE.Ideas.Core;
using CoE.Ideas.Remedy;
using CoE.Ideas.Remedy.RemedyServiceReference;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CoE.Ideas.EndToEnd.Tests
{
    internal class IntegrationRemedyService : RemedyService
    {
        public IntegrationRemedyService(New_Port_0PortType remedyClient,
             IOptions<RemedyServiceOptions> option,
             Serilog.ILogger logger) : base(remedyClient, option, logger)
        {
            WorkOrdersAdded = new List<RemedyWorkOrder>();
        }

        public ICollection<RemedyWorkOrder> WorkOrdersAdded { get; internal set; }

        public override async Task<string> PostNewIdeaAsync(Idea idea, string user3and3)
        {
            var returnValue = await base.PostNewIdeaAsync(idea, user3and3);
            WorkOrdersAdded.Add(new RemedyWorkOrder() { WorkOrderId = returnValue, Idea = idea });
            return returnValue;
        }

    }
}

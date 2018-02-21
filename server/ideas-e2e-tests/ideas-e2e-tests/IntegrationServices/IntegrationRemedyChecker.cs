using CoE.Ideas.Core.People;
using CoE.Ideas.Core.ServiceBus;
using CoE.Ideas.Remedy.Watcher;
using CoE.Ideas.Remedy.Watcher.RemedyServiceReference;
using Microsoft.Extensions.Options;
using Serilog;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CoE.Ideas.EndToEnd.Tests.IntegrationServices
{
    internal class IntegrationRemedyChecker : RemedyChecker
    {
        public IntegrationRemedyChecker(IRemedyService remedyService, 
            IInitiativeMessageSender initiativeMessageSender,
            IPeopleService peopleService,
            ILogger logger, 
            IOptions<RemedyCheckerOptions> options) 
            : base(remedyService, initiativeMessageSender, peopleService, logger, options)
        {
            WorkOrdersProcessed = new List<OutputMapping1GetListValues>();
            ProcessingErrors = new List<Tuple<OutputMapping1GetListValues, Exception>>();
        }

        public ICollection<OutputMapping1GetListValues> WorkOrdersProcessed;
        public ICollection<Tuple<OutputMapping1GetListValues, Exception>> ProcessingErrors;

        protected override async Task<Exception> TryProcessWorkItemChanged(OutputMapping1GetListValues workItem)
        {
            Exception error = await base.TryProcessWorkItemChanged(workItem);
            if (error == null)
            {
                // this is success
                WorkOrdersProcessed.Add(workItem);
            }
            else
            {
                ProcessingErrors.Add(new Tuple<OutputMapping1GetListValues, Exception>(workItem, error));
            }

            return error;
        }

    }
}

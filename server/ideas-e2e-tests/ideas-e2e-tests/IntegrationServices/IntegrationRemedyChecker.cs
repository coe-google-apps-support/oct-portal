using CoE.Ideas.Core.ServiceBus;
using CoE.Ideas.Remedy.Watcher;
using CoE.Ideas.Remedy.Watcher.RemedyServiceReference;
using CoE.Ideas.Shared.People;
using Microsoft.Extensions.Options;
using Serilog;
using System;
using System.Collections.Generic;
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
            WorkOrdersProcessed = new List<WorkOrderUpdatedEventArgs>();
            ProcessingErrors = new List<Tuple<OutputMapping1GetListValues, Exception>>();
        }

        public ICollection<WorkOrderUpdatedEventArgs> WorkOrdersProcessed;
        public ICollection<Tuple<OutputMapping1GetListValues, Exception>> ProcessingErrors;

        protected override async Task<WorkOrderUpdatedEventArgs> TryProcessWorkItemChanged(OutputMapping1GetListValues workItem)
        {
            Exception error = null;
            WorkOrderUpdatedEventArgs result = null;
            try
            {
                result = await base.TryProcessWorkItemChanged(workItem);
            }
            catch (Exception err)
            {
                error = err;
            }

            if (error == null)
            {
                // this is success
                WorkOrdersProcessed.Add(result);
            }
            else
            {
                ProcessingErrors.Add(new Tuple<OutputMapping1GetListValues, Exception>(workItem, error));
            }

            return result;
        }


        internal Task SimlateBusinessAnalystAssigned(string workdOrderId, string user3and3)
        {
            if (string.IsNullOrWhiteSpace(workdOrderId))
                throw new ArgumentNullException("workOrderId");
            if (string.IsNullOrWhiteSpace(user3and3))
                throw new ArgumentNullException(user3and3);

            return TryProcessWorkItemChanged(new OutputMapping1GetListValues()
            {
                InstanceId = workdOrderId,
                ASLOGID = user3and3,
                Last_Modified_Date = DateTime.Now,
                Status = StatusType.Assigned
            });
        }
    }
}

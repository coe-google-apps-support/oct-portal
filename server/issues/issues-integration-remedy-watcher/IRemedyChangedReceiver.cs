using CoE.Issues.Core.Remedy;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CoE.Issues.Remedy.Watcher
{
    public interface IRemedyChangedReceiver
    {
        void ReceiveChanges(DateTime fromDateUtc,
            Func<Incident, CancellationToken, Task> incidentUpdatedHandler = null,
            RemedyReaderOptions options = null);
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CoE.Issues.Core.ServiceBus
{
    public interface IIssueMessageSender
    {
        Task SendIssueCreatedAsync(IssueCreatedEventArgs args);
        Task SendNewIssueCreatedAsync(NewIssueCreatedEventArgs args);

    }
}

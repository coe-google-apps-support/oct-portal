using CoE.Issues.Core.Data;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CoE.Issues.Core.Services
{
    public interface IIssueRepository
    {
        Task<Issue> AddIssueAsync(Issue issue, CancellationToken cancellationToken = default(CancellationToken));
        Task<Issue> UpdateIssueAsync(Issue ssue);
        Task<Issue> GetIssueAsync(Guid id);
        Task<Issue> GetIssueAsync(int id);

    }
}
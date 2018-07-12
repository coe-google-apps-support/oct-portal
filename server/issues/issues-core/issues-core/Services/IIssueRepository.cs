using CoE.Ideas.Shared.Data;
using CoE.Issues.Core.Data;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CoE.Issues.Core.Services
{
    public interface IIssueRepository
    {
        Task<Issue> AddIssueAsync(Issue issue, CancellationToken cancellationToken = default(CancellationToken));
        Task<Issue> UpdateIssueAsync(Issue issue, CancellationToken cancellationToken = default(CancellationToken));
        Task<Issue> GetIssueAsync(Guid id);
        Task<PagedResultSet<IssueInfo>> GetIssuesAsync(string filter = null,
            int page = 1, int pageSize = 20);

        Task<Issue> GetIssueAsync(int id);
    }
}
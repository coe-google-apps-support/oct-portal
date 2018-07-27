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
        Task<Issue> DeleteIssueAsync(Issue issue, CancellationToken cancellationToken = default(CancellationToken));

        Task<Issue> GetIssueByIncidentIdAsync(string incidentId);
        Task<Issue> UpdateIssueAsync(Issue ssue);
        Task<Issue> GetIssueAsync(Guid id);
        Task<PagedResultSet<IssueInfo>> GetIssuesAsync(string filter = null,
            int page = 1, int pageSize = 20);
        Task<PagedResultSet<IssueInfo>> GetIssuesByStakeholderPersonIdAsync(int personId,
    string filter = null, int pageNumber = 1, int pageSize = 20);
        Task<Issue> GetIssueAsync(int id);
    }
}
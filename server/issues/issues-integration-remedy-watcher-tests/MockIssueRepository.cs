using CoE.Ideas.Shared.Data;
using CoE.Issues.Core.Data;
using CoE.Issues.Core.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CoE.Issues.Remedy.Watcher.Tests
{
    class MockIssueRepository : IIssueRepository
    {
        public Task<Issue> AddIssueAsync(Issue issue, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        public Task<Issue> GetIssueAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<Issue> GetIssueAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<PagedResultSet<IssueInfo>> GetIssuesAsync(string filter = null, int page = 1, int pageSize = 20)
        {
            throw new NotImplementedException();
        }

        public Task<Issue> UpdateIssueAsync(Issue ssue)
        {
            throw new NotImplementedException();
        }
    }
}

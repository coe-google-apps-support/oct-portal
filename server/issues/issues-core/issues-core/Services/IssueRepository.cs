using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CoE.Ideas.Shared;
using CoE.Issues.Core.Data;

namespace CoE.Issues.Core.Services
{
    internal class IssueRepository : IIssueRepository, IHealthCheckable
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

        public Task<IDictionary<string, object>> HealthCheckAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Issue> UpdateIssueAsync(Issue ssue)
        {
            throw new NotImplementedException();
        }
    }
}

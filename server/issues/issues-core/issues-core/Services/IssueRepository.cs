using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CoE.Ideas.Shared;
using CoE.Ideas.Shared.Data;
using CoE.Issues.Core.Data;
using EnsureThat;
using Microsoft.EntityFrameworkCore;

namespace CoE.Issues.Core.Services
{
    internal class IssueRepository : IIssueRepository, IHealthCheckable
    {
        public IssueRepository(IssueContext context)
        {
            EnsureArg.IsNotNull(context);
            _issueContext = context;
        }

        private readonly IssueContext _issueContext;

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

        public Task<PagedResultSet<Issue>> GetIssuesAsync(string filter = null, int pageNumber = 1, int pageSize = 20)
        {
            var issues = CreateIssueInfoQuery(_issueContext.Issues, filter);
            return PagedResultSet.Create(issues, pageNumber, pageSize);
        }

        private IQueryable<Issue> CreateIssueInfoQuery(IQueryable<Issue> query, string filter)
        {
            IQueryable<Issue> returnValue = query; // eager load any related entities here

            if (!string.IsNullOrWhiteSpace(filter))
            {
                returnValue = returnValue
                    .Where(x => x.Title.Contains(filter) || x.Description.Contains(filter));
            }

            return returnValue
                .OrderByDescending(x => x.CreatedDate);
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

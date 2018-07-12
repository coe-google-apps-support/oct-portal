using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CoE.Ideas.Shared;
using CoE.Ideas.Shared.Data;
using CoE.Issues.Core.Data;
using EnsureThat;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Serilog;

namespace CoE.Issues.Core.Services
{
    internal class LocalIssueRepository : IIssueRepository, IHealthCheckable
    {
        public LocalIssueRepository(IssueContext issuecontext,
            Serilog.ILogger logger)
        {
            EnsureArg.IsNotNull(issuecontext);
            EnsureArg.IsNotNull(logger);

            _issueContext = issuecontext;
            _logger = logger;
        }

        public void SetUser(ClaimsPrincipal user)
        {
            _user = user;
        }

        private readonly IssueContext _issueContext;
        private readonly Serilog.ILogger _logger;
        private ClaimsPrincipal _user;

        public async Task<Issue> AddIssueAsync(Issue issue,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            EnsureArg.IsNotNull(issue);

            _logger.Debug("Adding to Ideas database");
            try
            {
                _issueContext.Issues.Add(issue);
                await _issueContext.SaveChangesAsync(cancellationToken);
            }
            catch (Exception err)
            {
                _logger.Error(err, "Unable to add issue to database: {ErrorMessage}", err.Message);
                throw;
            }
            return issue;
        }

    public async Task<Issue> GetIssueAsync(Guid id)
        {
            var returnValue = await _issueContext.Issues
                .SingleOrDefaultAsync(x => x.Uid == id);
            // inefficient but safe:
            await _issueContext.Entry(returnValue).ReloadAsync();
            return returnValue;
        }

        public async Task<Issue> GetIssueAsync(int id)
        {
            var returnValue = await _issueContext.Issues
                .SingleOrDefaultAsync(x => x.Id == id);
            // inefficient but safe:
            await _issueContext.Entry(returnValue).ReloadAsync();
            return returnValue;
        }

        public async Task<PagedResultSet<IssueInfo>> GetIssuesAsync(string filter = null, int pageNumber = 1, int pageSize = 20)
        {
            var issues = CreateIssueInfoQuery(_issueContext.Issues, filter);
            return await PagedResultSet.Create(issues, pageNumber, pageSize);
        }

        private IQueryable<IssueInfo> CreateIssueInfoQuery(IQueryable<Issue> query, string filter)
        {
            IQueryable<Issue> returnValue = query; // eager load any related entities here

            if (!string.IsNullOrWhiteSpace(filter))
            {
                returnValue = returnValue
                    .Where(x => x.Title.Contains(filter) || x.Description.Contains(filter) || x.AssigneeEmail.Contains(filter) || x.RequestorName.Contains(filter) || x.ReferenceId.Contains(filter) || x.RemedyStatus.Contains(filter) );
                    
            }

            return returnValue
            .OrderByDescending(x => x.CreatedDate)
            .Select(x => IssueInfo.Create(x));


        }

        public Task<IDictionary<string, object>> HealthCheckAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Issue> UpdateIssueAsync(Issue issue,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        internal class IssueContractResolver : DefaultContractResolver
        {
            protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
            {
                var prop = base.CreateProperty(member, memberSerialization);
                if (!prop.Writable)
                {
                    var property = member as PropertyInfo;
                    if (property != null)
                    {
                        var hasPrivateSetter = property.GetSetMethod(true) != null;
                        prop.Writable = hasPrivateSetter;
                    }
                }
                return prop;
            }


        }

       
    }
}

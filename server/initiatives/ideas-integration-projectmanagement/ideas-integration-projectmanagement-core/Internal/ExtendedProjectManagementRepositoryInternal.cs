using AutoMapper;
using CoE.Ideas.Core;
using CoE.Ideas.Core.ProjectManagement;
using CoE.Ideas.ProjectManagement.Core.GitHub;
using CoE.Ideas.ProjectManagement.Core.Internal.GitHub;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoE.Ideas.ProjectManagement.Core.Internal
{
    internal class ExtendedProjectManagementRepositoryInternal : ProjectManagementRepositoryInternal, IExtendedProjectManagementRepository
    {
        private readonly ExtendedProjectManagementContext _context;
        private readonly IMapper _mapper;

        public ExtendedProjectManagementRepositoryInternal(ExtendedProjectManagementContext context,
            IMapper mapper) : base(context, mapper)
        {
            _context = context ?? throw new ArgumentNullException("context");
            _mapper = mapper ?? throw new ArgumentNullException("mapper");
        }

        #region Issues
        public async Task<Issue> DeleteIssueAsync(long id)
        {
            if (id <= 0)
                throw new ArgumentOutOfRangeException("id", "id cannot be less than or equal to zero");

            var issue = await _context.Issues.SingleOrDefaultAsync(m => m.Id == id);
            if (issue == null)
            {
                return null;
            }
            else
            {
                _context.Issues.Remove(issue);
                await _context.SaveChangesAsync();

                return _mapper.Map<IssueInternal, Issue>(issue);
            }
        }
        #endregion


        #region Issue Status Changes
        private async Task<IssueStatusChange> AddIssueStatusChangeAsync(IssueStatusChange issueStatusChange)
        {
            if (issueStatusChange == null)
                throw new ArgumentNullException("issueStatusChange");

            var issueStatusChangeInternal = _mapper.Map<IssueStatusChange, IssueStatusChangeInternal>(issueStatusChange);

            _context.IssueStatusChanges.Add(issueStatusChangeInternal);
            await _context.SaveChangesAsync();

            var returnValue = _mapper.Map<IssueStatusChangeInternal, IssueStatusChange>(issueStatusChangeInternal);

            return returnValue;
        }

        #endregion


        #region GitHub Issues

        public async Task<IEnumerable<GitHubIssue>> GetGitHubIssuesAsync()
        {
            //TODO: retrict to a reasonable amount of Issues
            var issues = await IssuesCollection.OfType<GitHubIssueInternal>().ToListAsync();
            return _mapper.Map<IEnumerable<GitHubIssueInternal>, IEnumerable<GitHubIssue>>(issues);

        }

        public async Task<GitHubIssue> GetGitHubIssueAsync(long id)
        {
            if (id <= 0)
                throw new ArgumentOutOfRangeException("id", "id cannot be less than or equal to zero");

            var issueInternal = await IssuesCollection.OfType<GitHubIssueInternal>().SingleOrDefaultAsync(m => m.Id == id);
            if (issueInternal == null)
                return null;
            else
                return _mapper.Map<GitHubIssueInternal, GitHubIssue>(issueInternal);
        }

        public async Task<GitHubIssueEvent> AddGitHubIssueEventAsync(GitHubIssueEvent issueEvent)
        {
            if (issueEvent == null)
                throw new ArgumentNullException("issueEvent");
            if (issueEvent.Issue == null)
                throw new ArgumentException("issueEvent.Issue cannot be null");

            var issueEventInternal = _mapper.Map<GitHubIssueEvent, GitHubIssueEventInternal>(issueEvent);

            // Action can come from either Action (WebHook) or from EventName (v3 API)
            if (!string.IsNullOrWhiteSpace(issueEvent.EventName) && string.IsNullOrWhiteSpace(issueEvent.Action))
                issueEvent.Action = issueEvent.EventName;

            _context.GitHubIssueEvents.Add(issueEventInternal);

            // This may trigger a github issue status change:
            IssueStatusChangeInternal statusChange = null;
            if (String.Equals(issueEvent.Action, "Closed", StringComparison.OrdinalIgnoreCase))
            {
                statusChange = new IssueStatusChangeInternal()
                {
                    Issue = issueEventInternal.Issue,
                    ChangeDate = DateTimeOffset.Now,
                    NewStatus = IssueStatusInternal.Closed
                };
            } else if (String.Equals(issueEvent.Action, "Opened", StringComparison.OrdinalIgnoreCase))
            {
                statusChange = new IssueStatusChangeInternal()
                {
                    Issue = issueEventInternal.Issue,
                    ChangeDate = DateTimeOffset.Now,
                    NewStatus = IssueStatusInternal.Open
                };
            }
            if (statusChange != null)
                _context.IssueStatusChanges.Add(statusChange);

            await _context.SaveChangesAsync();

            var returnValue = _mapper.Map<GitHubIssueEventInternal, GitHubIssueEvent>(issueEventInternal);

            return returnValue;
        }

        public async Task<GitHubIssue> UpdateGitHubIssueAsync(GitHubIssue issue)
        {
            if (issue == null)
                throw new ArgumentNullException("issue");

            var issueInternal = _mapper.Map<GitHubIssue, GitHubIssueInternal>(issue);
            _context.Entry(issueInternal).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await IssueExists(issue.Id))
                {
                    throw new EntityNotFoundException($"Issue with id {issue.Id} not found", "Issue", issue.Id);
                }
                else
                {
                    throw;
                }
            }

            return _mapper.Map<GitHubIssueInternal, GitHubIssue>(issueInternal);
        }
        #endregion

    }
}

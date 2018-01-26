using AutoMapper;
using CoE.Ideas.Core.ProjectManagement;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoE.Ideas.ProjectManagement.Core.Internal
{
    internal class ProjectManagementRepositoryInternal : IProjectManagementRepository
    {
        private readonly ProjectManagementContext _context;
        private readonly IMapper _mapper;

        public ProjectManagementRepositoryInternal(ProjectManagementContext context,
            IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException("context");
            _mapper = mapper ?? throw new ArgumentNullException("mapper");
        }

        #region Issues
        protected IQueryable<IssueInternal> IssuesCollection
        {
            get
            {
                // sorting 
                return _context.Issues
                    .OrderByDescending(x => x.CreatedDate);
          }
        }
        protected async Task<bool> IssueExists(long id)
        {
            return await _context.Issues.AnyAsync(e => e.Id == id);
        }



        public async Task<Issue> GetIssueAsync(long id)
        {
            if (id <= 0)
                throw new ArgumentOutOfRangeException("id", "id cannot be less than or equal to zero");

            var issueInternal = await IssuesCollection.SingleOrDefaultAsync(m => m.Id == id);
            if (issueInternal == null)
                return null;
            else
                return _mapper.Map<IssueInternal, Issue>(issueInternal);
        }

        public async Task<IEnumerable<Issue>> GetIssuesAsync()
        {
            //TODO: retrict to a reasonable amount of Issues
            var issues = await IssuesCollection.ToListAsync();
            return _mapper.Map<IEnumerable<IssueInternal>, IEnumerable<Issue>>(issues);
        }
        #endregion

        #region IssueStatusChanges
        private IQueryable<IssueStatusChangeInternal> IssueStatusChangesCollection
        {
            get
            {
                // sorting 
                return _context.IssueStatusChanges
                    .OrderByDescending(x => x.ChangeDate);
            }
        }


        public async Task<IEnumerable<IssueStatusChange>> GetIssueStatusChanges()
        {
            //TODO: retrict to a reasonable amount of Issues Status Changes
            var issueChanges = await IssueStatusChangesCollection.ToListAsync();
            return _mapper.Map<IEnumerable<IssueStatusChangeInternal>, IEnumerable<IssueStatusChange>>(issueChanges);
        }

        public async Task<IEnumerable<IssueStatusChange>> GetIssueStatusChanges(DateTimeOffset fromDate)
        {
            //TODO: retrict to a reasonable amount of Issues Status Changes
            var issueChanges = await IssueStatusChangesCollection
                .Where(x => x.ChangeDate >= fromDate).ToListAsync();
            return _mapper.Map<IEnumerable<IssueStatusChangeInternal>, IEnumerable<IssueStatusChange>>(issueChanges);
        }

        public async Task<IEnumerable<IssueStatusChange>> GetIssueStatusChanges(DateTimeOffset fromDate, DateTimeOffset toDate)
        {
            //TODO: retrict to a reasonable amount of Issues Status Changes
            var issueChanges = await IssueStatusChangesCollection
                .Where(x => x.ChangeDate >= fromDate && x.ChangeDate <= toDate).ToListAsync();
            return _mapper.Map<IEnumerable<IssueStatusChangeInternal>, IEnumerable<IssueStatusChange>>(issueChanges);
        }
        #endregion
    }
}

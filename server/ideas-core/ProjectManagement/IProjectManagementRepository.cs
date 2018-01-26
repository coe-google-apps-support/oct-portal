using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CoE.Ideas.Core.ProjectManagement
{
    public interface IProjectManagementRepository
    {
        #region Issues
        Task<IEnumerable<Issue>> GetIssuesAsync();

        Task<Issue> GetIssueAsync(long id);

        #endregion

        #region IssueChange
        Task<IEnumerable<IssueStatusChange>> GetIssueStatusChanges();
        Task<IEnumerable<IssueStatusChange>> GetIssueStatusChanges(DateTimeOffset fromDate);
        Task<IEnumerable<IssueStatusChange>> GetIssueStatusChanges(DateTimeOffset fromDate, DateTimeOffset toDate);
        #endregion
    }
}

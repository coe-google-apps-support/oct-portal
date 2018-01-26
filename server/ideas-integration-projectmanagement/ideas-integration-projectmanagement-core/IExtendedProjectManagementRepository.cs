using CoE.Ideas.Core.ProjectManagement;
using CoE.Ideas.ProjectManagement.Core.GitHub;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CoE.Ideas.ProjectManagement.Core
{
    public interface IExtendedProjectManagementRepository : IProjectManagementRepository
    {
        Task<IEnumerable<GitHubIssue>> GetGitHubIssuesAsync();
        Task<GitHubIssue> GetGitHubIssueAsync(long id);

        Task<GitHubIssue> UpdateGitHubIssueAsync(GitHubIssue issue);

        Task<GitHubIssueEvent> AddGitHubIssueEventAsync(GitHubIssueEvent issue);

        Task<Issue> DeleteIssueAsync(long id);
    }
}

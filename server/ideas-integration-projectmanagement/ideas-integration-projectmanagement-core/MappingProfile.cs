using AutoMapper.Configuration;
using CoE.Ideas.Core.ProjectManagement;
using CoE.Ideas.ProjectManagement.Core.GitHub;
using CoE.Ideas.ProjectManagement.Core.Internal;
using CoE.Ideas.ProjectManagement.Core.Internal.GitHub;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoE.Ideas.ProjectManagement.Core
{
    internal class MappingProfile : MapperConfigurationExpression
    {
        public MappingProfile() : base()
        {
            //"Regular" classes
            CreateMap<Issue, IssueInternal>();
            CreateMap<IssueInternal, Issue>();
            CreateMap<IssueStatus, IssueStatusInternal>();
            CreateMap<IssueStatusInternal, IssueStatus>();
            CreateMap<ProjectManagementSystem, ProjectManagementSystemInternal>();
            CreateMap<ProjectManagementSystemInternal, ProjectManagementSystem>();
            CreateMap<IssueStatusChange, IssueStatusChangeInternal>();
            CreateMap<IssueStatusChangeInternal, IssueStatusChange>();

            // PM provider specific classes:
            CreateMap<GitHubIssue, GitHubIssueInternal>();
            CreateMap<GitHubIssueInternal, GitHubIssue>();
            CreateMap<GitHubLabel, GitHubLabelInternal>();
            CreateMap<GitHubLabelInternal, GitHubLabel> ();
            CreateMap<GitHubRepository, GitHubRepositoryInternal>();
            CreateMap<GitHubRepositoryInternal, GitHubRepository>();
            CreateMap<GitHubUser, GitHubUserInternal>();
            CreateMap<GitHubUserInternal, GitHubUser>();
            CreateMap<GitHubIssueEvent, GitHubIssueEventInternal>();
            CreateMap<GitHubIssueEventInternal, GitHubIssueEvent>();
        }
    }
}

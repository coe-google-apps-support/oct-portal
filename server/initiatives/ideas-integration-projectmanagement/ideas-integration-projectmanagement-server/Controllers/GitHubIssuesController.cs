using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoE.Ideas.ProjectManagement.Core;
using CoE.Ideas.ProjectManagement.Core.GitHub;
using Microsoft.AspNetCore.Mvc;

namespace CoE.Ideas.ProjectManagement.Server.Controllers
{
    [Route("github/issues")]
    public class GitHubIssuesController : Controller
    {
        private readonly IExtendedProjectManagementRepository _repository;

        public GitHubIssuesController(IExtendedProjectManagementRepository repository)
        {
            _repository = repository;
        }


        // GET github/issues
        [HttpGet]
        public Task<IEnumerable<GitHubIssue>> Get()
        {
            return _repository.GetGitHubIssuesAsync();
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public Task<GitHubIssue> Get(int id)
        {
            return _repository.GetGitHubIssueAsync(id);
        }

        // POST api/values
        [HttpPost]
        public async Task Post([FromBody]GitHubIssueEvent value)
        {
            await _repository.AddGitHubIssueEventAsync(value);
        }
    }
}

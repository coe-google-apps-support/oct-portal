using CoE.Ideas.ProjectManagement.Core;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoE.Ideas.ProjectManagement.Server.Controllers
{
    [Route("issues")]
    public class IssuesController : Controller
    {
        private readonly IProjectManagementRepository _repository;

        public IssuesController(IProjectManagementRepository repository)
        {
            _repository = repository;
        }


        // GET github/issues
        [HttpGet]
        public Task<IEnumerable<Issue>> Get()
        {
            return _repository.GetIssuesAsync();
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public Task<Issue> Get(int id)
        {
            return _repository.GetIssueAsync(id);
        }
    }
}

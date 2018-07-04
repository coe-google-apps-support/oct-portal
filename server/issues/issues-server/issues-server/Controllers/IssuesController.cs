using CoE.Issues.Core.Services;
using CoE.Issues.Server.Models;
using EnsureThat;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using CoE.Ideas.Shared.Data;

namespace CoE.Issues.Server.Controllers
{
    [Produces("application/json")]
    //[Route("api")]
    [Route("api/[controller]")]
    public class IssuesController : Controller
    {
        private readonly IIssueRepository _repository;
        private readonly Serilog.ILogger _logger;

        public IssuesController(IIssueRepository repository,
            Serilog.ILogger logger)
        {
            EnsureArg.IsNotNull(repository);
            EnsureArg.IsNotNull(logger);
            _repository = repository;
            _logger = logger;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetIssues([FromQuery]ViewOptions view = ViewOptions.All,
            [FromQuery]string contains = null,
            [FromQuery]int page = 1,
            [FromQuery]int pageSize = 1000)
        {
            _logger.Information("Retrieving issues");

            if (page < 1)
            {
                ModelState.AddModelError(nameof(page), "page cannot be less than or equal to zero");
                return BadRequest(ModelState);
            }
            if (pageSize < 1)
            {
                ModelState.AddModelError(nameof(pageSize), "pageSize cannot be less than or equal to zero");
                return BadRequest(ModelState);
            }

            PagedResultSet<Core.Data.IssueInfo> issuesInfo;

            Stopwatch watch = new Stopwatch();
            watch.Start();
            try
            {
                if (view == ViewOptions.Mine)
                {
                    throw new NotSupportedException();
                    //issuesInfo = await _repository.GetissuesByStakeholderPersonIdAsync(User.GetPersonId(),
                    //    filter: contains, pageNumber: page, pageSize: pageSize);
                }
                else
                    issuesInfo = await _repository.GetIssuesAsync(filter: contains, page: page, pageSize: pageSize);
                watch.Stop();
                _logger.Information("Retrieved {IssueCount} issues in {ElapsedMilliseconds}ms", issuesInfo.ResultCount, watch.ElapsedMilliseconds);
                Request.HttpContext.Response.Headers.Add("X-Total-Count", issuesInfo.TotalCount.ToString());
                Request.HttpContext.Response.Headers.Add("X-Is-Last-Page", issuesInfo.IsLastPage().ToString());
                return Ok(issuesInfo.Results
                    .OrderByDescending(x => x.CreatedDate)
                    .Select(x => new IssueInfo()
                    {
                        Id = x.Id,
                        Description = x.Description,
                        Title = x.Title,
                        CreatedDate = x.CreatedDate,
                    }));
            }
            catch (Exception err)
            {
                _logger.Error(err, "Error reading issues: {ErrorMessage}", err.Message);
                throw;
            }
        }

    }
}

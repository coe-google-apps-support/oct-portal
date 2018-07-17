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
using CoE.Ideas.Shared.Security;
using CoE.Issues.Core.Data;

namespace CoE.Issues.Server.Controllers
{
    [Produces("application/json")]
    [Route("api")]

    //[Route("api")]
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
                    issuesInfo = await _repository.GetIssuesByStakeholderPersonIdAsync(User.GetPersonId(),
                        filter: contains, pageNumber: page, pageSize: pageSize);
                }
                else
                    issuesInfo = await _repository.GetIssuesAsync(filter: contains, page: page, pageSize: pageSize);
                watch.Stop();
                _logger.Information("Retrieved {IssueCount} issues in {ElapsedMilliseconds}ms", issuesInfo.ResultCount, watch.ElapsedMilliseconds);
                Request.HttpContext.Response.Headers.Add("X-Total-Count", issuesInfo.TotalCount.ToString());
                Request.HttpContext.Response.Headers.Add("X-Is-Last-Page", issuesInfo.IsLastPage().ToString());
                return Ok(issuesInfo.Results
                    .OrderByDescending(x => x.CreatedDate)
                    .Select(x => new Models.IssueInfo()
                    {
                        Id = x.Id,
                        Description = x.Description,
                        Title = x.Title,
                        CreatedDate = x.CreatedDate,
                        AssigneeEmail = x.AssigneeEmail,
                        RequestorName = x.RequestorName,
                        RemedyStatus = x.RemedyStatus,
                        ReferenceId = x.ReferenceId,
                    }));
            }
            catch (Exception err)
            {
                _logger.Error(err, "Error reading issues: {ErrorMessage}", err.Message);
                throw;
            }
        }


        [HttpPost]
        [Authorize]
        public async Task<IActionResult> PostIssue([FromBody] AddIssueDto issueData, bool skipEmailNotification = false)
        {
            EnsureArg.IsNotNull(issueData);

            if (!ModelState.IsValid)
            {
                _logger.Warning("Unable to create Issue because model state is not valid: {ModelState}", ModelState);
                return BadRequest(ModelState);
            }

            EnsureArg.IsNotNull(issueData.Title);
            EnsureArg.IsNotNull(issueData.Description);


            _logger.Information("Creating new Issue");
            Stopwatch watch = new Stopwatch();
            watch.Start();
            Issue newIssue = null;
            try
            {
                int personId = User.GetPersonId();

                newIssue = Issue.Create(issueData.Title, issueData.Description, "-1","-1", "-1", "-1", DateTime.Now, -1);

                newIssue = await _repository.AddIssueAsync(newIssue);

                watch.Stop();
                _logger.Information("Created Issue in {ElapsedMilliseconds}ms", watch.ElapsedMilliseconds);
                return CreatedAtAction("GetIdea", new { id = newIssue.Id }, newIssue);
            }
            catch (Exception err)
            {
                Guid correlationId = Guid.NewGuid();
                _logger.Error(err, "Unable to save new Issue {Issue} to repository. CorrelationId: {CorrelationId}", newIssue, correlationId);
#if DEBUG
                return base.StatusCode(500, $"Unable to save idea to repository. Error: { err }");
#else
                return base.StatusCode(500, $"Unable to save idea to repository. CorrelationId: { correlationId }");
# endif
            }

        }


    }
}

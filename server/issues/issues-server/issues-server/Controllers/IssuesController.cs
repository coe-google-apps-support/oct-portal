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
using Serilog.Context;

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
                //issuesInfo = await _repository.GetIssuesByStakeholderPersonIdAsync(User.GetPersonId(),
                 //   filter: contains, pageNumber: page, pageSize: pageSize);
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
                        AssigneeGroup = x.AssigneeGroup,
                        RequestorName = x.RequestorName,
                        RemedyStatus = x.RemedyStatus,
                        ReferenceId = x.ReferenceId,
                        Urgency = x.Urgency,
                    }));
            }
            catch (Exception err)
            {
                _logger.Error(err, "Error reading issues: {ErrorMessage}", err.Message);
                throw;
            }
        }


        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetIssue([FromRoute] string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                ModelState.AddModelError("id", "id cannot be empty");
                return BadRequest(ModelState);
            }
            else
            {
                if (!int.TryParse(id, out int initiativeId))
                {
                    _logger.Error($"id must be an integer if type is InitiativeKey, got { id }");
                    ModelState.AddModelError("id", "id must be an integer if type is InitiativeKey");
                    return BadRequest(ModelState);
                }
                else
                {
                    return await GetIssueByInitiativeId(initiativeId);
                }
            }
        }

        private async Task<IActionResult> GetIssueByInitiativeId(int id)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            try
            {
                return await ValidateAndGetIssue(id, initiative =>
                {
                    return Task.FromResult((IActionResult)Ok(initiative));
                });
            }
            finally
            {
                watch.Stop();
                _logger.Information("Retrieved initiative {InitiativeId} in {ElapsedMilliseconds}ms", id, watch.ElapsedMilliseconds);
            };
        }


        private async Task<IActionResult> ValidateAndGetIssue(int id, Func<Issue, Task<IActionResult>> callback)
        {
            using (LogContext.PushProperty("InitiativeId", id))
            {
                _logger.Information("Retrieving initiative {InitiativeId}", id);

                if (!ModelState.IsValid)
                {
                    _logger.Warning("Unable to get initiative {InitiativeId} because model state is not valid");
                    return BadRequest(ModelState);
                }

                var initiative = await _repository.GetIssueAsync(id);

                if (initiative == null)
                    return NotFound();

                return await callback(initiative);
            }
        }


        public DateTime ConvertTimeToAlberta(DateTime utctime)
        {
            TimeZoneInfo albertaTimeZone;
            try { albertaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("America/Edmonton"); }
            catch (TimeZoneNotFoundException)
            {
                _logger.Error("Unable to find Mountain Standard Time zone");
                throw;
            }
            var nowAlberta = TimeZoneInfo.ConvertTimeFromUtc(utctime, albertaTimeZone);

            return nowAlberta;


        }



    }
}

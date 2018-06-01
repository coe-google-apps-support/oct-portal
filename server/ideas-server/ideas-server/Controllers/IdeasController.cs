using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CoE.Ideas.Core.ServiceBus;
using System.Diagnostics;
using Serilog.Context;
using CoE.Ideas.Server.Models;
using CoE.Ideas.Core.Data;
using CoE.Ideas.Shared.Security;
using CoE.Ideas.Core.Services;
using EnsureThat;
using Microsoft.Extensions.Options;
using CoE.Ideas.Shared.Data;
using System.Net.Http;

namespace CoE.Ideas.Server.Controllers
{
    [Produces("application/json")]
    [Route("api")]
    public class IdeasController : Controller
    {
        private readonly IInitiativeRepository _repository;
        private readonly IPersonRepository _personRepository;
        private readonly IStringTemplateService _stringTemplateService;
        private readonly IInitiativeService _initiativeService;
        private readonly Serilog.ILogger _logger;

        public IdeasController(IInitiativeRepository repository,
            IPersonRepository personRepository,
            IStringTemplateService stringTemplateService,
            IInitiativeService initiativeService,
            Serilog.ILogger logger)
        {
            EnsureArg.IsNotNull(repository);
            EnsureArg.IsNotNull(personRepository);
            EnsureArg.IsNotNull(stringTemplateService);
            EnsureArg.IsNotNull(initiativeService);
            EnsureArg.IsNotNull(logger);

            _repository = repository;
            _personRepository = personRepository;
            _stringTemplateService = stringTemplateService;
            _initiativeService = initiativeService;
            _logger = logger;
        }

        // GET: ideas
        /// <summary>
        /// Retrieves all of the ideas 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetInitiatives([FromQuery]ViewOptions view = ViewOptions.All, 
            [FromQuery]string contains = null,
            [FromQuery]int page = 1, 
            [FromQuery]int pageSize = 1000)
        {
            _logger.Information("Retrieving Initiatives");

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

            PagedResultSet<Core.Data.InitiativeInfo> ideasInfo;

            Stopwatch watch = new Stopwatch();
            watch.Start();
            try
            {
                if (view == ViewOptions.Mine)
                {
                    ideasInfo = await _repository.GetInitiativesByStakeholderPersonIdAsync(User.GetPersonId(), 
                        filter: contains, pageNumber: page, pageSize: pageSize);
                }
                else
                    ideasInfo = await _repository.GetInitiativesAsync(filter: contains, page: page, pageSize: pageSize);
                watch.Stop();
                _logger.Information("Retrieved {InitiativesCount} Initiatives in {ElapsedMilliseconds}ms", ideasInfo.ResultCount, watch.ElapsedMilliseconds);
                Request.HttpContext.Response.Headers.Add("X-Total-Count", ideasInfo.TotalCount.ToString());
                Request.HttpContext.Response.Headers.Add("X-Is-Last-Page", ideasInfo.IsLastPage().ToString());
                return Ok(ideasInfo.Results
                    .OrderByDescending(x => x.CreatedDate)
                    .Select(x => new Models.InitiativeInfo()
                    {
                        Id = x.Id,
                        Description = x.Description,
                        Title = x.Title,
                        CreatedDate = x.CreatedDate,
                        Url = _initiativeService.GetInitiativeUrl(x.Id).ToString()
                    }));
            }
            catch (Exception err)
            {
                _logger.Error(err, "Error reading initiatives: {ErrorMessage}", err.Message);
                throw;
            }
        }

        // GET: ideas/5
        /// <summary>
        /// Retrieves a single Idea based on its Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetIdea([FromRoute] string id)
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
                    return await GetInitiativeByInitiativeId(initiativeId);
                }
            }
        }

        private async Task<IActionResult> GetInitiativeByInitiativeId(int id)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            try
            {
                return await ValidateAndGetInitiative(id, initiative =>
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

        // POST: ideas
        /// <summary>
        /// Adds a new Idea to the database
        /// </summary>
        /// <param name="idea"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> PostInitiative([FromBody] AddInitiativeDto initiativeData, bool skipEmailNotification = false)
        {
            EnsureArg.IsNotNull(initiativeData);

            if (!ModelState.IsValid)
            {
                _logger.Warning("Unable to create initiative because model state is not valid: {ModelState}", ModelState);
                return BadRequest(ModelState);
            }

            EnsureArg.IsNotNull(initiativeData.Title);
            EnsureArg.IsNotNull(initiativeData.Description);


            _logger.Information("Creating new initiative");
            Stopwatch watch = new Stopwatch();
            watch.Start();
            Initiative newInitiative = null;
            try
            {
                int personId = User.GetPersonId();

				newInitiative = Initiative.Create(initiativeData.Title, initiativeData.Description, personId, skipEmailNotification: skipEmailNotification);
                foreach (var supportingDocument in initiativeData.SupportingDocuments)
                {
                    var newSupportingDocuments = SupportingDocument.Create(supportingDocument.Title, supportingDocument.Url, supportingDocument.Type);
                    newInitiative.SupportingDocuments.Add(newSupportingDocuments);
                }
                newInitiative = await _repository.AddInitiativeAsync(newInitiative);

                watch.Stop();
                _logger.Information("Created initiative in {ElapsedMilliseconds}ms", watch.ElapsedMilliseconds);
                return CreatedAtAction("GetIdea", new { id = newInitiative.Id }, newInitiative);
            }
            catch (Exception err)
            {
                Guid correlationId = Guid.NewGuid();
                _logger.Error(err, "Unable to save new initiative {Initiative} to repository. CorrelationId: {CorrelationId}", newInitiative, correlationId);
#if DEBUG
                return base.StatusCode(500, $"Unable to save idea to repository. Error: { err }");
#else
                return base.StatusCode(500, $"Unable to save idea to repository. CorrelationId: { correlationId }");
# endif
            }

        }

        //// DELETE: ideas/5
        //[HttpDelete("{id}")]
        //[Authorize]
        //public async Task<IActionResult> DeleteIdea([FromRoute] int id)
        //{
        //    using (LogContext.PushProperty("InitiativeId", id))
        //    {
        //        Stopwatch watch = null;
        //        if (_logger.IsEnabled(Serilog.Events.LogEventLevel.Information))
        //        {
        //            _logger.Information("Deleting initiative {InitiativeId}");
        //            watch = new Stopwatch();
        //            watch.Start();
        //        }

        //        if (!ModelState.IsValid)
        //        {
        //            _logger.Warning("Unable to delete initiative {InitiativeId} because model state is not valid: {ModelState}", id, ModelState);
        //            return BadRequest(ModelState);
        //        }

        //        try
        //        {
        //            var idea = await _repository.DeleteIdeaAsync(id);
        //            if (idea == null)
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                if (_logger.IsEnabled(Serilog.Events.LogEventLevel.Information))
        //                {
        //                    watch.Stop();
        //                    _logger.Information("Updated initiative {InitiativeId} in {ElapsedMilliseconds}ms", id, watch.ElapsedMilliseconds);
        //                }
        //                return Ok(idea);
        //            }
        //        }
        //        catch (Exception err)
        //        {
        //            Guid correlationId = Guid.NewGuid();
        //            _logger.Error(err, "Unable to delete initiative InitiativeId from repository. CorrelationId: {CorrelationId}", id, correlationId);
        //            return base.StatusCode(500, $"Unable to delete initiative from repository. CorrelationId: { correlationId }");

        //        }

        //    }
        //}


        // GET: ideas/5/steps
        /// <summary>
        /// Retrieves a single Idea based on its Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}/steps")]
        public async Task<IActionResult> GetIdeaSteps([FromRoute] int id)
        {
            using (LogContext.PushProperty("InitiativeId", id))
            {
                Stopwatch watch = null;
                if (_logger.IsEnabled(Serilog.Events.LogEventLevel.Information))
                {
                    _logger.Information("Retrieving steps from initiative {InitiativeId}");
                    watch = new Stopwatch();
                    watch.Start();
                }

                return await ValidateAndGetInitiative(id, async initiative =>
                {
                    if (initiative.StatusHistories == null)
                        return NotFound();
                    else
                    {
                        var steps = await InitiativeStepDetail.FromInitiativeStatusHistoriesAsync(initiative.StatusHistories, _personRepository, _stringTemplateService);
                        watch.Stop();
                        _logger.Information("Retrieved steps for initiative {InitiativeId} in {ElapsedMilliseconds}ms", id, watch.ElapsedMilliseconds);
                        return Ok(steps);
                    }
                });
            }
        }

        // GET: ideas/5/resources
        /// <summary>
        /// Retrieves a single ideas resources based on it's id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}/resources")]
        public async Task<IActionResult> GetInitiativeResources([FromRoute] int id)
        {
            using (LogContext.PushProperty("InitiativeId", id))
            {
                Stopwatch watch = null;
                if (_logger.IsEnabled(Serilog.Events.LogEventLevel.Information))
                {
                    _logger.Information("Retrieving resources from initiative {InitiativeId}");
                    watch = new Stopwatch();
                    watch.Start();
                }

                try
                {
                    return await ValidateAndGetInitiative(id, async initiative =>
                    {
                        var steps = await InitiativeStepDetail.FromInitiativeStatusHistoriesAsync(initiative.StatusHistories, _personRepository, _stringTemplateService);

                        if (steps == null)
                            return NotFound();

                        var firstStep = steps.First();
                        if (firstStep.CompletionDate == null)
                            return NotFound();

                        var resources = new Resources();

                        if (initiative.AssigneeId.HasValue)
                        {
                            var assigneePerson = await _personRepository.GetPersonAsync(initiative.AssigneeId.Value);
                            if (assigneePerson != null)
                            {
                                resources.Assignee = new User()
                                {
                                    Email = assigneePerson.Email,
                                    Name = assigneePerson.Name,
                                    AvatarUrl = null,
                                    PhoneNumber = null
                                };
                            }
                        }
                        else
                        {
                            resources.Assignee = null;
                        }

                        return Ok(resources);
                    });
                }
                finally
                {
                    if (_logger.IsEnabled(Serilog.Events.LogEventLevel.Information))
                    {
                        watch.Stop();
                        _logger.Information("Retrieved resources for initiative {InitiativeId} in {ElapsedMilliseconds}ms", id, watch.ElapsedMilliseconds);
                    }


                }
            }
        }

        private async Task<IActionResult> ValidateAndGetInitiative(int id, Func<Initiative, Task<IActionResult>> callback)
        {
            using (LogContext.PushProperty("InitiativeId", id))
            {
                _logger.Information("Retrieving initiative {InitiativeId}", id);

                if (!ModelState.IsValid)
                {
                    _logger.Warning("Unable to get initiative {InitiativeId} because model state is not valid");
                    return BadRequest(ModelState);
                }

                var initiative = await _repository.GetInitiativeAsync(id);

                if (initiative == null)
                    return NotFound();

                return await callback(initiative);
            }
        }


        [HttpPut("{id}/statusDescription")]
        public async Task<IActionResult> UpdateStatusDescription(int id, [FromBody]UpdateStatusDescriptionDto updateStatusDescriptionDto)
        {
            EnsureArg.IsNotNull(updateStatusDescriptionDto);

            InitiativeStatus currentStatus;
            try { currentStatus = (InitiativeStatus)updateStatusDescriptionDto.StepId; }
            catch (InvalidCastException)
            {
                var initiativeStatusValues = Enum.GetValues(typeof(InitiativeStatus)).Cast<InitiativeStatus>();
                ModelState.AddModelError("stepId", $"stepId '{ updateStatusDescriptionDto.StepId }' is not a valid value, must be between { (int)initiativeStatusValues.First() } and { (int)initiativeStatusValues.Last() }");
                return base.BadRequest(ModelState);
            }

            return await ValidateAndGetInitiative(id, async initiative =>
            {
                if (initiative.Status != currentStatus)
                {
                    ModelState.AddModelError("stepId", $"stepId '{ updateStatusDescriptionDto.StepId }' is not the current state of the initiative ('{(int)initiative.Status}'). This is likely a concurrency issue. Please refresh and try again.");
                    return base.BadRequest(ModelState);
                }

                initiative.UpdateCurrentStatusDescription(updateStatusDescriptionDto.NewDescription);
                await _repository.UpdateInitiativeAsync(initiative);
                return Ok();
            });
        }

        [HttpPost("{id}/supportingdocuments")]
        public async Task<IActionResult> AddSupportingDocuments(int id, [FromBody]SupportingDocumentsDto supportingDocumentsDto)
        {
            EnsureArg.IsNotNull(supportingDocumentsDto);
            if (!ModelState.IsValid)
            {
                _logger.Warning("Unable to save supporting documents because model state is not valid: {ModelState}", ModelState);
                return BadRequest(ModelState);
            }

            return await ValidateAndGetInitiative(id, async initiative =>
            {
                if (User.IsAdmin() || IsCurrentUserAStakeholder(initiative))
                {
                    SupportingDocument newSupportingDocuments = null;
                    newSupportingDocuments = SupportingDocument.Create(supportingDocumentsDto.Title, supportingDocumentsDto.Url, supportingDocumentsDto.Type);
                    initiative.SupportingDocuments.Add(newSupportingDocuments);
                    await _repository.UpdateInitiativeAsync(initiative);
                    return Ok(newSupportingDocuments);
                }
                else
                {
                    return Unauthorized();
                }
            });
        }


        [HttpGet("{id}/supportingdocuments")]
        public async Task<IActionResult> GetSupportingDocuments([FromRoute] int id)
        {
            using (LogContext.PushProperty("InitiativeId", id))
            {
                Stopwatch watch = null;
                if (_logger.IsEnabled(Serilog.Events.LogEventLevel.Information))
                {
                    _logger.Information("Retrieving SupportingDocuments from initiative {InitiativeId}");
                    watch = new Stopwatch();
                    watch.Start();
                }

                try
                {
                    return await ValidateAndGetInitiative(id, initiative =>
                    {
                        if (User.IsAdmin() || IsCurrentUserAStakeholder(initiative))
                            Response.Headers.Add("Can-Edit", true.ToString());

                        return Task.FromResult((IActionResult)Ok(initiative.SupportingDocuments));
                    });
                }

                finally
                {
                    if (_logger.IsEnabled(Serilog.Events.LogEventLevel.Information))
                    {
                        watch.Stop();
                        _logger.Information("Retrieved supportingdocument for initiative {InitiativeId} in {ElapsedMilliseconds}ms", id, watch.ElapsedMilliseconds);
                    }


                }
            }
        }

        private bool IsCurrentUserAStakeholder(Initiative initiative)
        {
            EnsureArg.IsNotNull(initiative);
            if (initiative.Stakeholders == null)
                return false;
            int myId = User.GetPersonId();
            return (initiative.Stakeholders.Any(x => x.PersonId == myId));
        }


        //// GET: ideas/5
        ///// <summary>
        ///// Retrieves a single Idea based on its Id
        ///// </summary>
        ///// <param name="id"></param>
        ///// <returns></returns>
        //[HttpGet("{id}/assignee")]

        //public async Task<IActionResult> GetInitiativeAssignee([FromRoute] int id)
        //{
        //    using (LogContext.PushProperty("InitiativeId", id))
        //    {
        //        Stopwatch watch = null;
        //        if (_logger.IsEnabled(Serilog.Events.LogEventLevel.Information))
        //        {
        //            _logger.Information("Retrieving assignee for initiative {InitiativeId}");
        //            watch = new Stopwatch();
        //            watch.Start();
        //        }

        //        if (!ModelState.IsValid)
        //        {
        //            _logger.Warning("Unable to retrieve assignee from initiative {InitiativeId} because model state is not valid");
        //            return BadRequest(ModelState);
        //        }

        //        var idea = await _repository.GetInitiativeAsync(id);

        //        if (idea == null || !idea.AssigneeId.HasValue)
        //            return NotFound();

        //        var assigneePerson = await _personRepository.GetPersonAsync(idea.AssigneeId.Value);
        //        if (assigneePerson == null)
        //            return NotFound();

        //        // convert Person to user
        //        var assignee = new User()
        //        {
        //            Email = assigneePerson.Email,
        //            Name = assigneePerson.Name,
        //            AvatarUrl = null,
        //            PhoneNumber = null
        //        };

        //        if (_logger.IsEnabled(Serilog.Events.LogEventLevel.Information))
        //        {
        //            watch.Stop();
        //            _logger.Information("Retrieved assignee for initiative {InitiativeId} in {ElapsedMilliseconds}ms", id, watch.ElapsedMilliseconds);
        //        }

        //        return Ok(assignee);
        //    }
        //}

    }
}

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

namespace CoE.Ideas.Server.Controllers
{
    [Produces("application/json")]
    [Route("api")]
    public class IdeasController : Controller
    {
        private readonly IInitiativeRepository _repository;
        private readonly IPersonRepository _personRepository;
        private readonly IInitiativeMessageSender _initiativeMessageSender;
        private readonly Serilog.ILogger _logger;

        public IdeasController(IInitiativeRepository repository,
            IPersonRepository personRepository,
            IInitiativeMessageSender initiativeMessageSender,
            Serilog.ILogger logger)
        {
            _repository = repository ?? throw new ArgumentNullException("repository");
            _personRepository = personRepository ?? throw new ArgumentNullException("personRepository");
            _initiativeMessageSender = initiativeMessageSender ?? throw new ArgumentNullException("initiativeMessageSender");
            _logger = logger ?? throw new ArgumentNullException("logger");
        }

        // GET: ideas
        /// <summary>
        /// Retrieves all of the ideas 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public async Task<IEnumerable<Models.InitiativeInfo>> GetInitiatives([FromQuery]ViewOptions view = ViewOptions.All)
        {
            _logger.Information("Retrieving Initiatives");
            Stopwatch watch = new Stopwatch();
            watch.Start();

            IEnumerable<Core.Data.InitiativeInfo> ideas;
            if (view == ViewOptions.Mine)
            {
                ideas = await _repository.GetInitiativesByStakeholderPersonIdAsync(User.GetPersonId());
            }
            else
                ideas = await _repository.GetInitiativesAsync();
            var returnValue = ideas.OrderByDescending(x => x.CreatedDate);
            watch.Stop();
            _logger.Information("Retrieved {InitiativesCount} Initiatives in {ElapsedMilliseconds}ms", returnValue.Count(), watch.ElapsedMilliseconds);
            return returnValue.Select(x => new Models.InitiativeInfo()
            {
                Id = x.Id,
                Description = x.Description,
                Title = x.Title,
                CreatedDate = x.CreatedDate,
                Url = "https://octavadev.edmonton.ca/view-ideas/?id=" + x.Id //  $"{Request.Scheme}://{Request.Host}/view-ideas/?id={x.Id}"
            });
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
            using (LogContext.PushProperty("InitiativeId", id))
            {
                _logger.Information("Retrieving initiative {InitiativeId}");
                Stopwatch watch = new Stopwatch();
                watch.Start();

                if (id <= 0)
                {
                    _logger.Warning("Unable to retrieve initiative {InitiativeId} because id passed in was less than or equal to zero");
                    return BadRequest("id must be greater than zero");
                }
                else
                {
                    var idea = await _repository.GetInitiativeAsync(id);

                    if (idea == null)
                    {
                        _logger.Warning("Unable to find an initiative with id {InitiativeId}");
                        return NotFound();
                    }

                    watch.Stop();
                    _logger.Information("Retrieved initiative {InitiativeId} in {ElapsedMilliseconds}ms", id, watch.ElapsedMilliseconds);
                    return Ok(idea);
                }
            }
        }

        // PUT: ideas/5
        /// <summary>
        /// Modifies an Idea
        /// </summary>
        /// <param name="id"></param>
        /// <param name="idea"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutIdea([FromRoute] int id, [FromBody] Initiative idea)
        {
            using (LogContext.PushProperty("InitiativeId", id))
            {
                _logger.Information("Updating initiative {InitiativeId}");
                Stopwatch watch = new Stopwatch();
                watch.Start();

                if (!ModelState.IsValid)
                {
                    _logger.Warning("Unable to update initiative {InitiativeId} because model state is not valid: {ModelState}", id, ModelState);
                    return BadRequest(ModelState);
                }

                if (id != idea.Id)
                {
                    _logger.Warning("Unable to retrieve initiative {InitiativeId} because id of initiative retrieved from database was different than the id passed in");
                    return BadRequest();
                }

                try
                {
                    await _repository.UpdateInitiativeAsync(idea);

                    watch.Stop();
                    _logger.Information("Updated initiative {InitiativeId} in {ElapsedMilliseconds}ms", id, watch.ElapsedMilliseconds);
                }
                catch (Exception err)
                {
                    Guid correlationId = Guid.NewGuid();
                    _logger.Error(err, "Unable to save updated initiative {Initiative} to repository. CorrelationId: {CorrelationId}", idea, correlationId);
                    return base.StatusCode(500, $"Unable to save idea to repository. CorrelationId: { correlationId }");
                }

                return NoContent();
            }
        }

        // POST: ideas
        /// <summary>
        /// Adds a new Idea to the database
        /// </summary>
        /// <param name="idea"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> PostInitiative([FromBody] AddInitiativeDto initiativeData)
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

                newInitiative = Initiative.Create(initiativeData.Title, initiativeData.Description, personId);
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

                if (!ModelState.IsValid)
                {
                    _logger.Warning("Unable to retrieve steps from initiative {InitiativeId} because model state is not valid");
                    return BadRequest(ModelState);
                }

                var returnValue = await _repository.GetInitiativeStepsAsync(id);

                if (_logger.IsEnabled(Serilog.Events.LogEventLevel.Information))
                {
                    watch.Stop();
                    _logger.Information("Retrieved steps for initiative {InitiativeId} in {ElapsedMilliseconds}ms", id, watch.ElapsedMilliseconds);
                }

                if (returnValue == null)
                    return NotFound();
                else
                    return Ok(returnValue);
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

                if (!ModelState.IsValid)
                {
                    _logger.Warning("Unable to retrieve resources from initiative {InitiativeId} because model state is not valid");
                    return BadRequest(ModelState);
                }

                var initiative = await _repository.GetInitiativeAsync(id);

                if (initiative == null)
                    return NotFound();

                var steps = await _repository.GetInitiativeStepsAsync(id);
                if (steps == null)
                    return NotFound();

                var firstStep = steps.First<InitiativeStep>();
                if (firstStep.CompletionDate == null)
                    return NotFound();

                var resources = new Resources();
                
                if (initiative.AssigneeId.HasValue)
                {
                    var assigneePerson = await _personRepository.GetPersonAsync(initiative.AssigneeId.Value);
                    resources.Assignee = new User()
                    {
                        Email = assigneePerson.Email,
                        Name = assigneePerson.Name,
                        AvatarUrl = null,
                        PhoneNumber = null
                    };                    
                }
                else
                {
                    resources.Assignee = null;
                }
                
                resources.BusinessCaseUrl = initiative.BusinessCaseUrl;

                if (_logger.IsEnabled(Serilog.Events.LogEventLevel.Information))
                {
                    watch.Stop();
                    _logger.Information("Retrieved resources for initiative {InitiativeId} in {ElapsedMilliseconds}ms", id, watch.ElapsedMilliseconds);
                }

                return Ok(resources);
            }
        }

        // GET: ideas/5
        /// <summary>
        /// Retrieves a single Idea based on its Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}/assignee")]

        public async Task<IActionResult> GetInitiativeAssignee([FromRoute] int id)
        {
            using (LogContext.PushProperty("InitiativeId", id))
            {
                Stopwatch watch = null;
                if (_logger.IsEnabled(Serilog.Events.LogEventLevel.Information))
                {
                    _logger.Information("Retrieving assignee for initiative {InitiativeId}");
                    watch = new Stopwatch();
                    watch.Start();
                }

                if (!ModelState.IsValid)
                {
                    _logger.Warning("Unable to retrieve assignee from initiative {InitiativeId} because model state is not valid");
                    return BadRequest(ModelState);
                }

                var idea = await _repository.GetInitiativeAsync(id);

                if (idea == null || !idea.AssigneeId.HasValue)
                    return NotFound();

                var assigneePerson = await _personRepository.GetPersonAsync(idea.AssigneeId.Value);
                if (assigneePerson == null)
                    return NotFound();

                // convert Person to user
                var assignee = new User()
                {
                    Email = assigneePerson.Email,
                    Name = assigneePerson.Name,
                    AvatarUrl = null,
                    PhoneNumber = null
                };

                if (_logger.IsEnabled(Serilog.Events.LogEventLevel.Information))
                {
                    watch.Stop();
                    _logger.Information("Retrieved assignee for initiative {InitiativeId} in {ElapsedMilliseconds}ms", id, watch.ElapsedMilliseconds);
                }

                return Ok(assignee);
            }
        }

    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using CoE.Ideas.Core;
using CoE.Ideas.Core.ServiceBus;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using Serilog.Context;
using CoE.Ideas.Server.Models;
using CoE.Ideas.Core.WordPress;

namespace CoE.Ideas.Server.Controllers
{
    [Produces("application/json")]
    [Route("")]
    [Route("Ideas")]
    [Route("Initiatives")]
    public class IdeasController : Controller
    {
        private readonly IUpdatableIdeaRepository _repository;
        private readonly IWordPressClient _wordpressClient;
        private readonly IInitiativeMessageSender _initiativeMessageSender;
        private readonly Serilog.ILogger _logger;

        public IdeasController(IUpdatableIdeaRepository repository,
            IWordPressClient wordpressClient,
            IInitiativeMessageSender initiativeMessageSender,
            Serilog.ILogger logger)
        {
            _repository = repository ?? throw new ArgumentNullException("repository");
            _wordpressClient = wordpressClient ?? throw new ArgumentNullException("wordpressClient");
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
        public async Task<IEnumerable<Idea>> GetIdeas([FromQuery]ViewOptions view = ViewOptions.All)
        {
            _logger.Information("Retrieving Initiatives");
            Stopwatch watch = new Stopwatch();
            watch.Start();

            IEnumerable<Idea> ideas;
            if (view == ViewOptions.Mine)
            {
                ideas = await _repository.GetIdeasByStakeholderEmailAsync(User.GetEmail());
            }
            else
                ideas = await _repository.GetIdeasAsync();
            var returnValue = ideas.OrderByDescending(x => x.Id);
            watch.Stop();
            _logger.Information("Retrieved {InitiativesCount} Initiatives in {ElapsedMilliseconds}ms", returnValue.Count(), watch.ElapsedMilliseconds);
            return returnValue;
        }

        // GET: ideas/5
        /// <summary>
        /// Retrieves a single Idea based on its Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetIdea([FromRoute] string id, [FromQuery] InitiativeKeyType type = InitiativeKeyType.InitiativeKey)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                ModelState.AddModelError("id", "id cannot be empty");
                return BadRequest(ModelState);
            }
            else
            {
                if (type == InitiativeKeyType.InitiativeKey)
                {
                    if (!long.TryParse(id, out long initiativeId))
                    {
                        _logger.Error($"id must be an integer if type is InitiativeKey, got { id }");
                        ModelState.AddModelError("id", "id must be an integer if type is InitiativeKey");
                        return BadRequest(ModelState);
                    }
                    else
                    {
                        return await GetIdeaByInitiativeId(initiativeId);
                    }
                }
                else
                {
                    int wordPressKey;
                    if (type == InitiativeKeyType.Slug)
                    {
                        WordPressPost post;
                        try
                        {
                            post = await _wordpressClient.GetPostForInitativeSlug(id);
                        }
                        catch (Exception err)
                        {
                            _logger.Error(err, "Unable to get WordPress post id from slug {Slug}: {ErrorMessage}", id, err.Message);
                            throw;
                        }
                        if (post == null)
                            return NotFound();
                        else
                        {
                            wordPressKey = post.Id;
                            if (wordPressKey <= 0)
                                throw new InvalidOperationException($"Retrieved Post for slug { id } but the id was invalid ({post.Id})");
                            else
                                return await GetInitiativeByWordPresskey(wordPressKey);
                        }
                    }
                    else
                    {
                        if (int.TryParse(id, out wordPressKey))
                        {
                            return await GetInitiativeByWordPresskey(wordPressKey);
                        }
                        else
                        {
                            ModelState.AddModelError("id", "id must be an integer if type is WordPressPostKey");
                            return BadRequest(ModelState);
                        }
                    }
                }
            }
        }

        private async Task<IActionResult> GetIdeaByInitiativeId(long id)
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
                    var idea = await _repository.GetIdeaAsync(id);

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

        private async Task<IActionResult> GetInitiativeByWordPresskey(int wordPressKey)
        {
            using (LogContext.PushProperty("WordPressKey", wordPressKey))
            {
                _logger.Information("Retrieving initiative by WordPressKey {WordPressKey}");
                Stopwatch watch = new Stopwatch();
                watch.Start();

                if (wordPressKey <= 0)
                {
                    _logger.Warning("Unable to retrieve initiative with WordPressKey {WordPressKey} because id passed in was less than or equal to zero");
                    return BadRequest("id must be greater than zero");
                }
                else
                {
                    var idea = await _repository.GetIdeaByWordpressKeyAsync(wordPressKey);

                    if (idea == null)
                    {
                        _logger.Warning("Unable to find an initiative with WorkPressKey {WordPressKey}");
                        return NotFound();
                    }

                    watch.Stop();
                    _logger.Information("Retrieved initiative {WordPressKey} in {ElapsedMilliseconds}ms", wordPressKey, watch.ElapsedMilliseconds);
                    return Ok(idea);
                }
            }
        }


        // GET: ideas/wp/5
        /// <summary>
        /// Retrieves a single Idea based on its assocated wordpressKey
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("wp/{id}")]
        public async Task<IActionResult> GetIdeaByWordpressKey([FromRoute] int id)
        {
            using (LogContext.PushProperty("WordPressId", id))
            {
                _logger.Information("Retrieving initiative by WordPress id {WordPressId}");
                Stopwatch watch = new Stopwatch();
                watch.Start();
                if (!ModelState.IsValid)
                {
                    _logger.Warning("Unable to retrieve initiative by WordPress id {WordPressId} because model state is not valid");
                    return BadRequest(ModelState);
                }

                var idea = await _repository.GetIdeaByWordpressKeyAsync(id);

                if (idea == null)
                {
                    _logger.Warning("Unable to find an initiative with WordPress id {WordPressId}");
                    return NotFound();
                }

                watch.Stop();
                _logger.Information("Retrieved initiative {InitiativeId} from WordPress id {WordPressId} in {ElapsedMilliseconds}ms", idea.Id, id, watch.ElapsedMilliseconds);
                return Ok(idea);
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
        public async Task<IActionResult> PutIdea([FromRoute] long id, [FromBody] Idea idea)
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
                    await _repository.UpdateIdeaAsync(idea);

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
        public async Task<IActionResult> PostIdea([FromBody] Idea idea)
        {
            if (idea == null)
                throw new ArgumentNullException("idea");

            if (!ModelState.IsValid)
            {
                _logger.Warning("Unable to create initiative because model state is not valid: {ModelState}", ModelState);
                return BadRequest(ModelState);
            }

            _logger.Information("Creating new initiative");
            Stopwatch watch = new Stopwatch();
            watch.Start();
            try
            {
                // post to WordPress
                var wordPressIdeaTask = _wordpressClient.PostIdeaAsync(idea);

                var newIdeaTask = _repository.AddIdeaAsync(idea, User);

                await Task.WhenAll(newIdeaTask, wordPressIdeaTask);

                var updateIdeaTask = _repository.SetWordPressItemAsync(newIdeaTask.Result.Id, wordPressIdeaTask.Result);

                _logger.Information("Posting to service bus");
                var sendToServiceBusTask = _initiativeMessageSender.SendInitiativeCreatedAsync(
                    new InitiativeCreatedEventArgs()
                    {
                        Initiative =
                        newIdeaTask.Result,
                        Owner =
                        User
                    });

                await Task.WhenAll(updateIdeaTask, sendToServiceBusTask);
                _logger.Information("Posted to service bus");

                if (_logger.IsEnabled(Serilog.Events.LogEventLevel.Information))
                {
                    watch.Stop();
                    _logger.Information("Created initiative in {ElapsedMilliseconds}ms", watch.ElapsedMilliseconds);
                }
                return CreatedAtAction("GetIdea", new { id = newIdeaTask.Result.Id }, newIdeaTask.Result);
            }
            catch (Exception err)
            {
                Guid correlationId = Guid.NewGuid();
                _logger.Error(err, "Unable to save new initiative {Initiative} to repository. CorrelationId: {CorrelationId}", idea, correlationId);
#if DEBUG
                return base.StatusCode(500, $"Unable to save idea to repository. Error: { err }");
#else
                return base.StatusCode(500, $"Unable to save idea to repository. CorrelationId: { correlationId }");
# endif
            }

        }

        // DELETE: ideas/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteIdea([FromRoute] long id)
        {
            using (LogContext.PushProperty("InitiativeId", id))
            {
                Stopwatch watch = null;
                if (_logger.IsEnabled(Serilog.Events.LogEventLevel.Information))
                {
                    _logger.Information("Deleting initiative {InitiativeId}");
                    watch = new Stopwatch();
                    watch.Start();
                }

                if (!ModelState.IsValid)
                {
                    _logger.Warning("Unable to delete initiative {InitiativeId} because model state is not valid: {ModelState}", id, ModelState);
                    return BadRequest(ModelState);
                }

                try
                {
                    var idea = await _repository.DeleteIdeaAsync(id);
                    if (idea == null)
                    {
                        return NotFound();
                    }
                    else
                    {
                        if (_logger.IsEnabled(Serilog.Events.LogEventLevel.Information))
                        {
                            watch.Stop();
                            _logger.Information("Updated initiative {InitiativeId} in {ElapsedMilliseconds}ms", id, watch.ElapsedMilliseconds);
                        }
                        return Ok(idea);
                    }
                }
                catch (Exception err)
                {
                    Guid correlationId = Guid.NewGuid();
                    _logger.Error(err, "Unable to delete initiative InitiativeId from repository. CorrelationId: {CorrelationId}", id, correlationId);
                    return base.StatusCode(500, $"Unable to delete initiative from repository. CorrelationId: { correlationId }");

                }

            }
        }




        // GET: ideas/5/steps
        /// <summary>
        /// Retrieves a single Idea based on its Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}/steps")]
        public IActionResult GetIdeaSteps([FromRoute] long id)
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

                //var idea = await _repository.GetIdeaAsync(id);

                //if (idea == null)
                //{
                //    return NotFound();
                //}

                // TODO: replace fake data with real data
                var fakeData = Newtonsoft.Json.Linq.JObject.Parse(fakeSteps);
                var returnValue = Json(fakeData);

                if (_logger.IsEnabled(Serilog.Events.LogEventLevel.Information))
                {
                    watch.Stop();
                    _logger.Information("Retrieved steps for initiative {InitiativeId} in {ElapsedMilliseconds}ms", id, watch.ElapsedMilliseconds);
                }

                return returnValue;
            }
        }

        // GET: ideas/5
        /// <summary>
        /// Retrieves a single Idea based on its Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}/assignee")]

        public async Task<IActionResult> GetInitiativeAssignee([FromRoute] long id)
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

                var idea = await _repository.GetIdeaAsync(id);

                if (idea == null || idea.Assignee == null)
                    return NotFound();

                // convert Person to user
                var assignee = new User()
                {
                    Email = idea.Assignee.Email,
                    Name = idea.Assignee.UserName,
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

#region Fake Data

        private const string fakeSteps = @"{
  data: [{
    step: 1,
    name: 'Submit',
    status: 'done',
    completedDate: 'Jan 14 2018 9:17:00 GMT-0700 (Mountain Standard Time)',
    type: 'text',
    data: 'Congrats! You submitted this on January 14th, 2018.'
  },
  {
    step: 2,
    name: 'Review',
    status: 'done',
    completedDate: 'Jan 14 2018 11:17:00 GMT-0700 (Mountain Standard Time)',
    type: 'resource',
    data: [{
      user: 'super.ba@edmonton.ca',
      assignedOn: 'Jan 14 2018 10:55:32 GMT-0700 (Mountain Standard Time)',
      avatarURL: '/wp-content/plugins/coe-ideas/assets/avatar/avatar1.png'
    }
    ]
  },
  {
    step: 3,
    name: 'Collaborate',
    status: 'done',
    completedDate: 'Jan 18 2018 11:17:00 GMT-0700 (Mountain Standard Time)',
    type: 'resource',
    data: [{
      user: 'super.ba@edmonton.ca',
      assignedOn: 'Jan 14 2018 12:31:55 GMT-0700 (Mountain Standard Time)',
      avatarURL: '/wp-content/plugins/coe-ideas/assets/avatar/avatar1.png'
    }
    ]
  },
  {
    step: 4,
    name: 'Deliver',
    status: 'ongoing',
    completedDate: null,
    type: 'burndown',
    url: 'https://github.com/coe-google-apps-support/oct-portal',
    initialWork: 24,
    data: [
      {
        date: 'Jan 18 2018 12:00:00 GMT-0700 (Mountain Standard Time)',
        workAdded: 0,
        workRemoved: 0,
        url: 'https://github.com/search?utf8=%E2%9C%93&q=repo%3Acoe-google-apps-support%2Foct-portal+closed%3A2018-01-18..2018-01-18&type=Issues'
      },
      {
        date: 'Jan 19 2018 12:00:00 GMT-0700 (Mountain Standard Time)',
        workAdded: 0,
        workRemoved: 2,
        url: 'https://github.com/search?utf8=%E2%9C%93&q=repo%3Acoe-google-apps-support%2Foct-portal+closed%3A2018-01-19..2018-01-19&type=Issues'
      },
      {
        date: 'Jan 20 2018 12:00:00 GMT-0700 (Mountain Standard Time)',
        workAdded: 0,
        workRemoved: 1,
        url: 'https://github.com/search?utf8=%E2%9C%93&q=repo%3Acoe-google-apps-support%2Foct-portal+closed%3A2018-01-20..2018-01-20&type=Issues'
      },
      {
        date: 'Jan 21 2018 12:00:00 GMT-0700 (Mountain Standard Time)',
        workAdded: 0,
        workRemoved: 1,
        url: 'https://github.com/search?utf8=%E2%9C%93&q=repo%3Acoe-google-apps-support%2Foct-portal+closed%3A2018-01-21..2018-01-21&type=Issues'
      },
      {
        date: 'Jan 22 2018 12:00:00 GMT-0700 (Mountain Standard Time)',
        workAdded: 0,
        workRemoved: 1,
        url: 'https://github.com/search?utf8=%E2%9C%93&q=repo%3Acoe-google-apps-support%2Foct-portal+closed%3A2018-01-22..2018-01-22&type=Issues'
      },
      {
        date: 'Jan 23 2018 12:00:00 GMT-0700 (Mountain Standard Time)',
        workAdded: 0,
        workRemoved: 1,
        url: 'https://github.com/search?utf8=%E2%9C%93&q=repo%3Acoe-google-apps-support%2Foct-portal+closed%3A2018-01-23..2018-01-23&type=Issues'
      },
      {
        date: 'Jan 24 2018 12:00:00 GMT-0700 (Mountain Standard Time)',
        workAdded: 0,
        workRemoved: 3,
        url: 'https://github.com/search?utf8=%E2%9C%93&q=repo%3Acoe-google-apps-support%2Foct-portal+closed%3A2018-01-24..2018-01-24&type=Issues'
      },
      {
        date: 'Jan 25 2018 12:00:00 GMT-0700 (Mountain Standard Time)',
        workAdded: 0,
        workRemoved: 3,
        url: 'https://github.com/search?utf8=%E2%9C%93&q=repo%3Acoe-google-apps-support%2Foct-portal+closed%3A2018-01-25..2018-01-25&type=Issues'
      },
      {
        date: 'Jan 26 2018 12:00:00 GMT-0700 (Mountain Standard Time)',
        workAdded: 0,
        workRemoved: 3,
        url: 'https://github.com/search?utf8=%E2%9C%93&q=repo%3Acoe-google-apps-support%2Foct-portal+closed%3A2018-01-26..2018-01-26&type=Issues'
      },
      {
        date: 'Jan 27 2018 12:00:00 GMT-0700 (Mountain Standard Time)',
        workAdded: 0,
        workRemoved: 3,
        url: 'https://github.com/search?utf8=%E2%9C%93&q=repo%3Acoe-google-apps-support%2Foct-portal+closed%3A2018-01-27..2018-01-27&type=Issues'
      },
      {
        date: 'Jan 28 2018 12:00:00 GMT-0700 (Mountain Standard Time)',
        workAdded: 0,
        workRemoved: 3,
        url: 'https://github.com/search?utf8=%E2%9C%93&q=repo%3Acoe-google-apps-support%2Foct-portal+closed%3A2018-01-28..2018-01-28&type=Issues'
      },
      {
        date: 'Jan 29 2018 12:00:00 GMT-0700 (Mountain Standard Time)',
        workAdded: 23,
        workRemoved: 0,
        url: 'https://github.com/search?utf8=%E2%9C%93&q=repo%3Acoe-google-apps-support%2Foct-portal+closed%3A2018-01-29..2018-01-29&type=Issues'
      },
      {
        date: 'Jan 30 2018 12:00:00 GMT-0700 (Mountain Standard Time)',
        workAdded: 1,
        workRemoved: 3,
        url: 'https://github.com/search?utf8=%E2%9C%93&q=repo%3Acoe-google-apps-support%2Foct-portal+closed%3A2018-01-30..2018-01-30&type=Issues'
      },
      {
        date: 'Jan 31 2018 12:00:00 GMT-0700 (Mountain Standard Time)',
        workAdded: 0,
        workRemoved: 5,
        url: 'https://github.com/search?utf8=%E2%9C%93&q=repo%3Acoe-google-apps-support%2Foct-portal+closed%3A2018-01-31..2018-02-31&type=Issues'
      }
    ]
  }
  ]
}";
#endregion

    }
}
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

namespace CoE.Ideas.Server.Controllers
{
    [Produces("application/json")]
    [Route("Ideas")]
    [Route("Initiatives")]
    public class IdeasController : Controller
    {
        private readonly IIdeaRepository _repository;
        private readonly ILogger<IdeasController> _logger;

        public IdeasController(IIdeaRepository repository,
            ILogger<IdeasController> logger)
        {
            _repository = repository ?? throw new ArgumentNullException("repository");
            _logger = logger ?? throw new ArgumentNullException("logger");
        }

        // GET: ideas
        /// <summary>
        /// Retrieves all of the ideas 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IEnumerable<Idea>> GetIdeas()
        {
            var ideas = await _repository.GetIdeasAsync();
            return ideas.OrderByDescending(x => x.Id);
        }

        // GET: ideas/5
        /// <summary>
        /// Retrieves a single Idea based on its Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetIdea([FromRoute] long id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var idea = await _repository.GetIdeaAsync(id);

            if (idea == null)
            {
                return NotFound();
            }

            return Ok(idea);
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
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var idea = await _repository.GetIdeaByWordpressKeyAsync(id);

            if (idea == null)
            {
                return NotFound();
            }

            return Ok(idea);
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
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != idea.Id)
            {
                return BadRequest();
            }

            await _repository.UpdateIdeaAsync(idea);

            return NoContent();
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
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Stopwatch watch = null;
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug("Posting idea");
                watch = new Stopwatch();
                watch.Start();
            }
            var newIdea = await _repository.AddIdeaAsync(idea);

            if (_logger.IsEnabled(LogLevel.Debug))
            {
                watch.Stop();
                _logger.LogDebug($"Posted idea in { watch.ElapsedMilliseconds }ms");
            }

            return CreatedAtAction("GetIdea", new { id = newIdea.Id }, newIdea);
        }

        // DELETE: ideas/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteIdea([FromRoute] long id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var idea = await _repository.DeleteIdeaAsync(id);
            if (idea == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(idea);
            }
        }




        // GET: ideas/5
        /// <summary>
        /// Retrieves a single Idea based on its Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}/steps")]
        public IActionResult GetIdeaSteps([FromRoute] long id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //var idea = await _repository.GetIdeaAsync(id);

            //if (idea == null)
            //{
            //    return NotFound();
            //}

            // TODO: replace fake data with real data
            var fakeData = Newtonsoft.Json.Linq.JObject.Parse(fakeSteps);
            return Json(fakeData);
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
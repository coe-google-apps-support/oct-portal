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

namespace CoE.Ideas.Server.Controllers
{
    [Produces("application/json")]
    [Route("Ideas")]
    [Route("Initiatives")]
    public class IdeasController : Controller
    {
        private readonly IIdeaRepository _repository;

        public IdeasController(IIdeaRepository repository)
        {
            _repository = repository;
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

            var newIdea = await _repository.AddIdeaAsync(idea);

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
    type: 'text',
    data: 'Congrats! You submitted this on January 14th, 2018.'
  },
  {
    step: 2,
    name: 'Review',
    status: 'done',
    type: 'chat',
    data: [{
      user: 'super.ba@edmonton.ca',
      created: 'Jan 14 2018 10:55:32 GMT-0700 (Mountain Standard Time)',
      data: 'Wow. What a great idea. Just consulting with my BA side-kick, bat-ba.'
    },
    {
      user: 'bat.ba@edmonton.ca',
      created: 'Jan 14 2018 11:07:00 GMT-0700 (Mountain Standard Time)',
      data: 'Agreed. Game changer.'
    },
    {
      user: 'super.ba@edmonton.ca',
      created: 'Jan 14 2018 11:11:00 GMT-0700 (Mountain Standard Time)',
      data: 'All systems go!!'
    }
    ]
  },
  {
    step: 3,
    name: 'Collaborate',
    status: 'done',
    type: 'chat',
    data: [{
      user: 'a.random@edmonton.ca',
      created: 'Jan 16 2018 7:03:32 GMT-0700 (Mountain Standard Time)',
      data: 'how do I get in on this? seems like this could have some potential'
    },
    {
      user: 'super.ba@edmonton.ca',
      created: 'Jan 16 2018 8:07:00 GMT-0700 (Mountain Standard Time)',
      data: 'If you\'d like, we could add you to the list of stakeholders?'
    },
    {
      user: 'a.random@edmonton.ca',
      created: 'Jan 17 2018 11:11:00 GMT-0700 (Mountain Standard Time)',
      data: 'ya please do that'
    },
    {
      user: 'gregory.onuczko@edmonton.ca',
      created: 'Jan 17 2018 11:19:00 GMT-0700 (Mountain Standard Time)',
      data: 'Glad to have you onboard, A Random!!'
    },
    {
      user: 'super.ba@edmonton.ca',
      created: 'Jan 18 2018 8:07:00 GMT-0700 (Mountain Standard Time)',
      data: 'Transitioning to our technical experts for solutioning.'
    }
    ]
  },
  {
    step: 4,
    name: 'Deliver',
    status: 'ongoing',
    type: 'burndown',
    initialWork: 24,
    data: [{
      date: 'Jan 23 2018 12:00:00 GMT-0700 (Mountain Standard Time)',
      workAdded: 0,
      workRemoved: 3,
      url: 'https://github.com/search?utf8=%E2%9C%93&q=repo%3Acoe-google-apps-support%2Foct-portal+closed%3A2018-01-23..2018-01-24&type=Issues'
    },
    {
      date: 'Jan 24 2018 12:00:00 GMT-0700 (Mountain Standard Time)',
      workAdded: 0,
      workRemoved: 3,
      url: 'https://github.com/search?utf8=%E2%9C%93&q=repo%3Acoe-google-apps-support%2Foct-portal+closed%3A2018-01-24..2018-01-25&type=Issues'
    },
    {
      date: 'Jan 25 2018 12:00:00 GMT-0700 (Mountain Standard Time)',
      workAdded: 0,
      workRemoved: 3,
      url: 'https://github.com/search?utf8=%E2%9C%93&q=repo%3Acoe-google-apps-support%2Foct-portal+closed%3A2018-01-25..2018-01-26&type=Issues'
    },
    {
      date: 'Jan 26 2018 12:00:00 GMT-0700 (Mountain Standard Time)',
      workAdded: 0,
      workRemoved: 3,
      url: 'https://github.com/search?utf8=%E2%9C%93&q=repo%3Acoe-google-apps-support%2Foct-portal+closed%3A2018-01-26..2018-01-27&type=Issues'
    },
    {
      date: 'Jan 27 2018 12:00:00 GMT-0700 (Mountain Standard Time)',
      workAdded: 0,
      workRemoved: 3,
      url: 'https://github.com/search?utf8=%E2%9C%93&q=repo%3Acoe-google-apps-support%2Foct-portal+closed%3A2018-01-27..2018-01-28&type=Issues'
    }
    ]
  }
  ]
}";
        #endregion

    }
}
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

    }
}
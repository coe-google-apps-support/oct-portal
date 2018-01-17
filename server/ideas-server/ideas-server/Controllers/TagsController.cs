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
    [Route("Tags")]
    public class TagsController : Controller
    {
        private readonly IIdeaRepository _repository;
        private readonly IIdeaServiceBusSender _serviceBusSender;

        public TagsController(IIdeaRepository repository,
            IIdeaServiceBusSender serviceBusSender)
        {
            _repository = repository;
            _serviceBusSender = serviceBusSender;
        }

        // GET: api/Tags
        /// <summary>
        /// Retrieves all of the Tags 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IEnumerable<Tag>> GetTags()
        {
            var tags = await _repository.GetTagsAsync();
            return tags.OrderByDescending(x => x.CreatedDate);
        }

        // GET: api/Ideas/5
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

        // PUT: api/Ideas/5
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

        // POST: api/Ideas
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
            await _serviceBusSender.SendIdeaCreatedMessageAsync(newIdea);

            return CreatedAtAction("GetIdea", new { id = newIdea.Id }, newIdea);
        }

        // DELETE: api/Ideas/5
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
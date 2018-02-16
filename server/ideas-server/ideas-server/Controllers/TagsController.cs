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
        private readonly IUpdatableIdeaRepository _repository;

        public TagsController(IUpdatableIdeaRepository repository)
        {
            _repository = repository;
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


    }
}
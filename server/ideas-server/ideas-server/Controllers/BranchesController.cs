using CoE.Ideas.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoE.Ideas.Server.Controllers
{
    [Produces("application/json")]
    [Route("Branches")]
    public class BranchesController : Controller
    {
        private readonly IIdeaRepository _repository;

        public BranchesController(IIdeaRepository repository)
        {
            _repository = repository;
        }

        // GET: branches
        /// <summary>
        /// Retrieves all of the branches 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IEnumerable<Branch>> GetBranches()
        {
            var branches = await _repository.GetBranchesAsync();
            return branches.OrderBy(b => b.Name);
        }

        // GET: branches/5
        /// <summary>
        /// Retrieves a single Branch based on its Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBranch([FromRoute] long id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var branch = await _repository.GetBranchAsync(id);

            if (branch == null)
            {
                return NotFound();
            }

            return Ok(branch);
        }

        // GET: departments
        /// <summary>
        /// Retrieves the depmartments for a given branch
        /// </summary>
        /// <returns></returns>
        [HttpGet("{branchId}/departments")]
        public async Task<IEnumerable<Department>> GetDepartments([FromRoute]long branchId)
        {
            var departments = await _repository.GetDepartmentsForBranchAsync(branchId);
            return departments.OrderBy(b => b.Name);
        }
    }
}

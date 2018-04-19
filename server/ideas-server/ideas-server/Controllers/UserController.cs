using CoE.Ideas.Core.Services;
using CoE.Ideas.Shared.Security;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoE.Ideas.Server.Controllers
{
    [Produces("application/json")]
    [Route("api/user")]
    public class UserController : Controller
    {
        private readonly IPersonRepository _personRepository;
        private readonly Serilog.ILogger _logger;

        public UserController(IPersonRepository personRepository,
            Serilog.ILogger logger)
        {
            _personRepository = personRepository;
            _logger = logger;
        }

        public async Task<IActionResult> GetCurrentUser()
        {
            if (!User.Identity.IsAuthenticated)
                return NotFound();
            else
            {
                string email;
                try
                {
                    email = User.GetEmail();
                }
                catch (InvalidOperationException err)
                {
                    return NotFound($"Unable to retrieve email address for current user '{User.Identity.Name}': {err.Message}");
                }

                if (string.IsNullOrWhiteSpace(email))
                {
                    return NotFound($"Email address is empty for current user '{User.Identity.Name}'");
                }

                var personId = await _personRepository.GetPersonIdByEmailAsync(email);
                if (personId <= 0)
                    return NotFound($"Unable to find a user for {email}");

                var person = await _personRepository.GetPersonAsync(personId);
                if (person == null)
                    return NotFound($"Unable to find a user for {email} with id {personId}");

                // cheating here a little....
                return Ok(new { person.Id, person.Name, person.Email, Roles = User.GetRoles().ToArray() });
            }
        }
    }
}

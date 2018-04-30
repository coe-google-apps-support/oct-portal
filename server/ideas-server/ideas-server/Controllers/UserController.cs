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
            {
                _logger.Warning("GetCurrentUser called for unauthenticated user");
                return NotFound();
            }
            else
            {
                string email;
                try
                {
                    email = User.GetEmail();
                }
                catch (InvalidOperationException err)
                {
                    _logger.Error(err, "Unable to retrieve email address for current user '{UserName}': {ErrorMessage}", User.Identity.Name, err.Message);
                    return NotFound($"Unable to retrieve email address for current user '{User.Identity.Name}': {err.Message}");
                }

                if (string.IsNullOrWhiteSpace(email))
                {
                    _logger.Error("Email address is empty for current user {UserName}", User.Identity.Name);
                    return NotFound($"Email address is empty for current user '{User.Identity.Name}'");
                }

                var personId = await _personRepository.GetPersonIdByEmailAsync(email);
                if (personId == null)
                {
                    _logger.Error("Unable to find a user for {EmailAddress}", email);
                    return NotFound($"Unable to find a user for {email}");
                }

                var person = await _personRepository.GetPersonAsync(personId.Value);
                if (person == null)
                {
                    _logger.Error("Unable to find a user for {EmailAddress} with id {PersonId}", email, personId);
                    return NotFound($"Unable to find a user for {email} with id {personId}");
                }

                // The following is temporary and should be replaced by real logic after our demo on tuesday.
                var permissions = new List<string>();
                if (User.IsAdmin() || User.IsInRole("Octava Business Analyst"))
                {
                    string permissionName = Core.Data.Permissions.EditStatusDescription.ToString();
                    permissionName = permissionName.First().ToString().ToLower() + permissionName.Substring(1);
                    permissions.Add(permissionName);
                }

                return Ok(new { person.Id, person.Name, person.Email, Roles = User.GetRoles().ToArray(), Permissions = permissions.ToArray() });
            }
        }
    }
}

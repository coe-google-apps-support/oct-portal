using CoE.Ideas.Core.Data;
using CoE.Ideas.Core.Services;
using EnsureThat;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CoE.Ideas.Server.Controllers
{
    [Produces("application/json")]
    [Route("api/statusEtas")]
    // This class is necessary for the InitiativeStatusEtaService, which can only be run internally.
    public class StatusEtaController : Controller
    {
        private readonly Serilog.ILogger _logger;
        private readonly IInitiativeStatusEtaRepository _initiativeStatusEtaRepository;

        public StatusEtaController(IInitiativeStatusEtaRepository initiativeStatusEtaRepository,
            Serilog.ILogger logger)
        {
            EnsureArg.IsNotNull(initiativeStatusEtaRepository);
            EnsureArg.IsNotNull(logger);
            _logger = logger;
            _initiativeStatusEtaRepository = initiativeStatusEtaRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetStatusEtas()
        {
            IEnumerable<StatusEta> returnValue = await _initiativeStatusEtaRepository.GetStatusEtasAsync();

            if (returnValue == null)
                return NotFound(returnValue);
            else
                return Ok(returnValue);
        }
    }
}

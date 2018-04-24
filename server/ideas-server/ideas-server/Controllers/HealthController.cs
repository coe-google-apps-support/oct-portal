using CoE.Ideas.Shared;
using EnsureThat;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoE.Ideas.Server.Controllers
{
    [Produces("application/json")]
    [Route("api/health")]
    public class HealthController : Controller
    {
        public HealthController(IServiceProvider serviceProvider)
        {
            EnsureArg.IsNotNull(serviceProvider);
            _serviceProvider = serviceProvider;
        }

        private readonly IServiceProvider _serviceProvider;

        [HttpGet]
        public async Task<IActionResult> HealthCheck()
        {
            var svcs = _serviceProvider.GetService(typeof(IEnumerable<IHealthCheckable>)) as IEnumerable<IHealthCheckable>;
            string finalResult;
            var svcResults = new List<IDictionary<string, object>>();
            if (svcs == null || !svcs.Any())
                finalResult = "No HealthCheckable services found";
            else
            {
                bool anyError = false;
                foreach (var svc in svcs)
                {
                    Exception svcError = null;
                    IDictionary<string, object> checkResult = null;
                    try { checkResult = await svc.HealthCheckAsync(); }
                    catch (Exception err)
                    {
                        svcError = err;
                    }
                    if (checkResult == null /*|| !User.IsInRole("Administrator")*/) // uncomment before going to production
                        checkResult = new Dictionary<string, object>();
                    checkResult["name"] = svc.GetType().Name;
                    checkResult["fullName"] = svc.GetType().FullName;
                    if (svcError != null)
                    {
                        checkResult["result"] = svcError.Message;
                        anyError = true;
                    }
                    else
                        checkResult["result"] = "OK";
                    svcResults.Add(checkResult);
                }
                if (anyError)
                    finalResult = "One or more errors occurred";
                else
                    finalResult = "OK";
            }
            return Ok(new { Result = finalResult, Services = svcResults });
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoE.Ideas.Server.Controllers
{
    // based on https://stackoverflow.com/questions/42414397/asp-net-core-mvc-catch-all-route-serve-static-file
    // answer by Ben Anderson
    public class HomeController : Controller
    {
        public IActionResult Spa()
        {
            if (Request.Path.HasValue && Request.Path.Value.Contains("."))
            {
                // exception for index.html itself
                if (Request.Path.Value.Equals("/index.html",StringComparison.OrdinalIgnoreCase))
                    return File("~/index.html", "text/html");
                else
                    return base.NotFound();
            }
            else
                return File("~/index.html", "text/html");
        }
    }
}

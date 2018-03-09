using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoE.Ideas.Shared.Security.Events
{
    public class CookieReceivedContext : ResultContext<WordPressCookieAuthenticationOptions>
    {
        public CookieReceivedContext(HttpContext context,
            AuthenticationScheme scheme,
            WordPressCookieAuthenticationOptions options) : base(context, scheme, options)
        {

        }
    }
}

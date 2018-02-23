using Microsoft.AspNetCore.Authentication;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoE.Ideas.Core.Security
{
    public class WordPressCookieAuthenticationOptions : AuthenticationSchemeOptions
    {
        public string WordPressUrl { get; set; }
    }
}

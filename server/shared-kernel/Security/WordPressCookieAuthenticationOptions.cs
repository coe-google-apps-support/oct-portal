using Microsoft.AspNetCore.Authentication;

namespace CoE.Ideas.Shared.Security
{
    public class WordPressCookieAuthenticationOptions : AuthenticationSchemeOptions
    {
        public string WordPressUrl { get; set; }
    }
}

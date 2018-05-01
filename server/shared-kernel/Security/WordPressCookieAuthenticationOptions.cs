using Microsoft.AspNetCore.Authentication;

namespace CoE.Ideas.Shared.Security
{
    public class WordPressCookieAuthenticationOptions : AuthenticationSchemeOptions
    {
        public string WordPressUrl { get; set; }
        public string DevUserName { get; set; }
        public string DevUserEmail { get; set; }
    }
}

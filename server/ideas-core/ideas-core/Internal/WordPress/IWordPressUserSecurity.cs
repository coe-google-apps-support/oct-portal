using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CoE.Ideas.Core.Internal.WordPress
{
    internal interface IWordPressUserSecurity
    {
        Task<ClaimsPrincipal> AuthenticateUserAsync(string cookie, string scheme = "auth");

        void SetWordPressCredentials(HttpClient httpClient, CookieContainer cookieContainer);
        void SetWordPressCredentials(HttpClient httpClient, CookieContainer cookieContainer, ClaimsPrincipal user);
    }
}

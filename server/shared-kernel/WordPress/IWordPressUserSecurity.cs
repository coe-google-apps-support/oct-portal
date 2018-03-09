using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CoE.Ideas.Shared.WordPress
{
    public interface IWordPressUserSecurity
    {
        Task<ClaimsPrincipal> AuthenticateUserAsync(string cookie, string scheme = "auth");

        void SetWordPressCredentials(HttpClient httpClient, CookieContainer cookieContainer);
        void SetWordPressCredentials(HttpClient httpClient, CookieContainer cookieContainer, ClaimsPrincipal user);
    }
}

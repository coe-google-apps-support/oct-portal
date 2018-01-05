using CoE.Ideas.Core.WordPress;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CoE.Ideas.Core.WordPress
{
    internal class WordPressClient : IWordPressClient
    {
        // _httpContextAccessor is used to get the current user; set by Dependency Injection
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly Uri _wordPressUrl;

        public WordPressClient(IOptions<WordPressClientOptions> options, 
            IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            string wordPressUrl = options.Value.Url.ToString();
            if (!wordPressUrl.EndsWith("/"))
                wordPressUrl += "/";
            _wordPressUrl = new Uri(wordPressUrl);
        }

        public async Task<WordPressUser> GetCurrentUserAsync()
        {
            var userId = GetUserId(_httpContextAccessor.HttpContext.User);

            var client = GetHttpClient();

            try
            {
                // note we need context=edit to get additional fields, like email
                var wpUserString = await client.GetStringAsync($"users/{userId}?context=edit");
                return JsonConvert.DeserializeObject<WordPressUser>(wpUserString);
               
            }
            catch (Exception err)
            {
                throw err;
            }
        }

        protected static int GetUserId(ClaimsPrincipal principal)
        {
            int id;
            var idClaim = principal.FindFirst(ClaimTypes.NameIdentifier);

            if (idClaim == null)
            {
                throw new SecurityException("Unable to get id of current user");
            }
            else if (idClaim.ValueType != ClaimValueTypes.Integer)
            {
                throw new SecurityException("Unable to get id of current user, NameIdentifier claim is not of type Integer");
            }
            else
            {
                if (!int.TryParse(idClaim.Value, out id))
                    throw new SecurityException($"Unable to get id of current user, NameIdentifier claim is of type Integer but could not cast value '{idClaim.Value}' to an integer");

                return id;
            }
        }

        protected virtual HttpClient GetHttpClient()
        {
            var client = new HttpClient();

            // easy case - we have a bearer token in our own HTTP Request headers:
            // so we can just reuse it because WordPress should be using the same
            // JWT Auth keys we are.
            var requestHeaders = _httpContextAccessor.HttpContext?.Request?.Headers;
            string authToken = null;
            if (requestHeaders != null && requestHeaders.ContainsKey("Authorization"))
            {
                authToken = requestHeaders["Authorization"];
            }

            if (string.IsNullOrWhiteSpace(authToken))
            {
                throw new SecurityException("Unable to get current JWT Authorization token");
            }
            else
            {
                if (authToken.Length > 7 && authToken.StartsWith("Bearer "))
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authToken.Substring(7));
                else
                    throw new SecurityException("Unable to get current JWT Authorization token (does not contain 'Bearer' keyword)");
            }

            client.BaseAddress = new Uri(_wordPressUrl, "wp-json/wp/v2/");

            return client;
        }
    }
}

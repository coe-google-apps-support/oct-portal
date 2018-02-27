using CoE.Ideas.Core.Security;
using CoE.Ideas.Core.WordPress;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CoE.Ideas.Core.WordPress
{
    internal class WordPressClient : IWordPressClient
    {
        public WordPressClient(IOptions<WordPressClientOptions> options) : this(options, null)
        {
        }


        public WordPressClient(IOptions<WordPressClientOptions> options,
            IHttpContextAccessor httpContextAccessor)
        {
            if (options == null)
                throw new ArgumentNullException("options");
            string wordPressUrl = options.Value.Url.ToString();
            if (!wordPressUrl.EndsWith("/"))
                wordPressUrl += "/";
            _wordPressUrl = new Uri(wordPressUrl);

            _httpContextAccessor = httpContextAccessor; // allowed to be null
        }



        // _httpContextAccessor is used to get the current user; set by Dependency Injection
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly Uri _wordPressUrl;

        public ClaimsPrincipal User { get; set; }


        public async Task<WordPressUser> GetCurrentUserAsync()
        {
            using (var client = GetHttpClient())
            {
                try
                {
                    // note we need context=edit to get additional fields, like email
                    var wpUserString = await client.GetStringAsync($"users/me?context=edit");
                    return JsonConvert.DeserializeObject<WordPressUser>(wpUserString);

                }
                catch (Exception err)
                {
                    throw err;
                }
            }

        }


        public async Task<WordPressUser> GetUserAsync(int wordPressuserId)
        {
            using (var client = GetHttpClient())
            {
                try
                {
                    // note we need context=edit to get additional fields, like email
                    var wpUserString = await client.GetStringAsync($"users/{wordPressuserId}?context=edit");
                    return JsonConvert.DeserializeObject<WordPressUser>(wpUserString);

                }
                catch (Exception err)
                {
                    throw err;
                }
            }
        }


        public async Task<WordPressPost> PostIdeaAsync(Idea idea)
        {
            if (idea == null)
                throw new ArgumentNullException("idea");
            if (string.IsNullOrWhiteSpace(idea.Title))
                throw new ArgumentOutOfRangeException("idea cannot have an empty Title");

            // Note this requires that the "Ideas" custom Post Type has been already 
            // create in WordPress, and the option to include it in the REST API is also on.
            using (var client = GetHttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
               // client.DefaultRequestHeaders.Add("Content-Type", "application/json");
                try
                {
                    dynamic postdata = new Newtonsoft.Json.Linq.JObject();
                    postdata.title = idea.Title;
                    postdata.content = idea.Description;
                    postdata.status = "publish";
                    //postdata.template = "single-initiative.php";

                    var postResponse = await client.PostAsync("initiatives", new StringContent(postdata.ToString(), Encoding.UTF8, "application/json"));
                    var postResponseMessage = await postResponse.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<WordPressPost>(postResponseMessage);

                }
                catch (Exception err)
                {
                    throw err;
                }
            }
        }


        public async Task<WordPressPost> GetPostForInitativeSlug(string slug)
        {
            using (var client = GetHttpClient())
            {
                try
                {
                    // schema: https://developer.wordpress.org/rest-api/reference/posts/
                    var postInfoString = await client.GetStringAsync($"initiatives/slug={slug}");
                    return JsonConvert.DeserializeObject<WordPressPost>(postInfoString);
                }
                catch (Exception err)
                {
                    throw err;
                }
            }
        }

        //protected static int GetUserId(ClaimsPrincipal principal)
        //{
        //    int id;
        //    var idClaim = principal.FindFirst(ClaimTypes.NameIdentifier);

        //    if (idClaim == null)
        //    {
        //        throw new SecurityException("Unable to get id of current user");
        //    }
        //    else if (idClaim.ValueType != ClaimValueTypes.Integer)
        //    {
        //        throw new SecurityException("Unable to get id of current user, NameIdentifier claim is not of type Integer");
        //    }
        //    else
        //    {
        //        if (!int.TryParse(idClaim.Value, out id))
        //            throw new SecurityException($"Unable to get id of current user, NameIdentifier claim is of type Integer but could not cast value '{idClaim.Value}' to an integer");

        //        return id;
        //    }
        //}


        protected virtual HttpClient GetHttpClient()
        {

            var client = new HttpClient();

            string wordPressAuthCookieName = WordPressCookieAuthenticationHandler.GetWordPressCookieName(_wordPressUrl.ToString());

            var existingCookie = _httpContextAccessor.HttpContext?.Request?.Cookies?.LastOrDefault(x => x.Key == wordPressAuthCookieName);
            if (!existingCookie.HasValue || string.IsNullOrWhiteSpace(existingCookie.Value.Value) && User != null)
            {
                // create the cookie

                // cookie has form:  Name|Expiration|Hash;

                // TODO: generate a proper auth token
                DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
                TimeSpan span = DateTime.Now.AddMinutes(10).Subtract(epoch);
                long expiration = Convert.ToInt64(span.TotalSeconds);
                existingCookie = new KeyValuePair<string, string>(wordPressAuthCookieName, $"{User.Identity.Name}|{ expiration }");
            }

            var cookieContainer = new CookieContainer();
            if (!string.IsNullOrWhiteSpace(existingCookie?.Value))
                cookieContainer.Add(new Uri($"{ _wordPressUrl.Scheme }://{ _wordPressUrl.Host }"), new Cookie(wordPressAuthCookieName, existingCookie.Value.Value));

            client.BaseAddress = new Uri(_wordPressUrl, "wp-json/wp/v2/");

            return client;
        }

    }
}

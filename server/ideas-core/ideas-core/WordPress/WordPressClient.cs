using CoE.Ideas.Core.WordPress;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
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
        // _httpContextAccessor is used to get the current user; set by Dependency Injection
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly Uri _wordPressUrl;

        private string jwtCredentials;
        public string JwtCredentials
        {
            get
            {
                if (string.IsNullOrWhiteSpace(jwtCredentials) && 
                    _httpContextAccessor != null && 
                    _httpContextAccessor.HttpContext != null)
                {
                    var requestHeaders = _httpContextAccessor.HttpContext.Request?.Headers;
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
                            jwtCredentials = authToken.Substring(7);
                        else
                            throw new SecurityException("Unable to get current JWT Authorization token (does not contain 'Bearer' keyword)");
                    }

                }
                return jwtCredentials;
            }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    if (value.Length > 7 && value.StartsWith("Bearer", StringComparison.OrdinalIgnoreCase))
                    {
                        jwtCredentials = value.Substring(7);
                    }
                    else
                        jwtCredentials = value;
                }
                else
                    jwtCredentials = value;
            }
        }

        public WordPressClient(IOptions<WordPressClientOptions> options) : this(options, null)
        {

        }


        public WordPressClient(IOptions<WordPressClientOptions> options, 
            IHttpContextAccessor httpContextAccessor)
        {
            if (options == null)
                throw new ArgumentNullException("options");

            _httpContextAccessor = httpContextAccessor; // allowed to be null
            string wordPressUrl = options.Value.Url.ToString();
            if (!wordPressUrl.EndsWith("/"))
                wordPressUrl += "/";
            _wordPressUrl = new Uri(wordPressUrl);
        }

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

            if (string.IsNullOrWhiteSpace(JwtCredentials))
            {
                throw new SecurityException("JWTCredentials must be set or obtainable from HTTP Request headers");
            }

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", JwtCredentials);

            client.BaseAddress = new Uri(_wordPressUrl, "wp-json/wp/v2/");

            return client;
        }

    }
}

using CoE.Ideas.Core.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CoE.Ideas.Core.Internal.WordPress
{
    internal class WordPressUserSecurity : IWordPressUserSecurity
    {
        public WordPressUserSecurity(IHttpContextAccessor httpContextAccessor,
            WordPressContext wordPressContext,
            IOptions<WordPressUserSecurityOptions> options,
            Serilog.ILogger logger)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException("httpContextAccessor");
            _wordPressContext = wordPressContext ?? throw new ArgumentNullException("wordPressContext");
            _options = options?.Value ?? throw new ArgumentNullException("options");
            _logger = logger ?? throw new ArgumentNullException("logger");
        }

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly WordPressUserSecurityOptions _options;
        private readonly Serilog.ILogger _logger;
        private readonly WordPressContext _wordPressContext;

        const string CLAIM_TYPE_ID = "http://octavia.edmonton.ca/schemas/2018/02/id";

        #region Authentication (Incoming)
        public async Task<ClaimsPrincipal> AuthenticateUserAsync(string cookie, string scheme = "auth")
        {
            if (string.IsNullOrWhiteSpace(cookie))
                throw new ArgumentNullException("cookie");

            _logger.Information("Attempting to authenticate user using WordPress");
            var watch = new Stopwatch();
            watch.Start();

            string[] cookieParts = cookie.Split("|");
            if (cookieParts.Length < 3)
            {
                _logger.Error("Expected cookie to contain Name|Expiration|Hash|HMAC: {WordPressCookie}", cookie);
                throw new InvalidOperationException($"Expected cookie to contain Name|Expiration|Hash, but only got { cookieParts.Length } parts to the cookie");
            }
            else
            {
                if (!long.TryParse(cookieParts[1], out long expiration))
                {
                    _logger.Error("Unable to determine expiration of WordPress cookie as it was not a 64-bit integer: {WordPressCookie}", cookie);
                    throw new InvalidOperationException("Unable to determine expiration of WordPress cookie as it was not a 64-bit integer");
                }
                else
                {
                    DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
                    if (epoch.AddSeconds(expiration) <= DateTime.Now)
                    {
                        _logger.Error("WordPress cookie is expired: {WordPressCookie}", cookie);
                        throw new InvalidOperationException("WordPress cookie is expired");
                    }
                    else
                    {
                        try
                        {
                            string username = System.Web.HttpUtility.UrlDecode(cookieParts[0]);
                            _logger.Information("Authenticating user {UserName}", username);
                            var result = await VerifyHashAndCreatePrincipal(username, expiration, cookieHash: cookieParts[2], scheme: scheme);
                            watch.Stop();
                            _logger.Information("Authenticated user {UserName} in {ElapsedMilliseconds}", username, watch.ElapsedMilliseconds);
                            return result;
                        }
                        catch (Exception err)
                        {
                            _logger.Error(err, "Unknown error authenticating user: {ErrorMessage}", err.Message);
                            throw;
                        }
                    }
                }
            }
        }

        private async Task<ClaimsPrincipal> VerifyHashAndCreatePrincipal(string username, long expiration, string cookieHash, string scheme = "auth")
        {

            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentNullException("username");
            if (string.IsNullOrWhiteSpace(cookieHash))
                throw new ArgumentNullException("cookieHash");

            DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            if (epoch.AddSeconds(expiration) <= DateTime.Now)
                throw new ArgumentOutOfRangeException("expiration", "expiration is in the past");


            // WordPress cookies explained: https://www.securitysift.com/understanding-wordpress-auth-cookies/
            var userInfo = await _wordPressContext.Users.Where(x => x.UserName == username)
                .Select(x => new
                {
                    x.Id,
                    x.Email,
                    x.Password,
                    x.Url,
                    x.Name // display name
                })
                .SingleOrDefaultAsync();

            if (userInfo == null)
                throw new EntityNotFoundException($"Unable to find user with username { username }");

            var metaKeys = new string[] { "wp_capabilities", "first_name", "last_name" };
            var metadataInfoTask = _wordPressContext.UserMetadata
                .Where(x => x.UserId == userInfo.Id && metaKeys.Contains(x.Key))
                .Select(x => new { x.Key, x.Value })
                .ToListAsync();

            // TODO: implement cookie hash verification like WordPress
            //VeryifyCookieHash(cookieHash, expiration, username, userInfo.Password, scheme);

            var claims = new List<Claim>()
            {
                new Claim(CLAIM_TYPE_ID, userInfo.Id.ToString()),
                new Claim(ClaimTypes.NameIdentifier, username),
                new Claim(ClaimTypes.Email, userInfo.Email),
                new Claim(ClaimTypes.Name, userInfo.Name),
                new Claim(ClaimTypes.Uri, userInfo.Url)
            };

            // augment with user metadata
            var userMetadata = await metadataInfoTask;
            claims.Add(new Claim(ClaimTypes.GivenName, userMetadata.SingleOrDefault(x => x.Key == "first_name")?.Value));
            claims.Add(new Claim(ClaimTypes.Surname, userMetadata.SingleOrDefault(x => x.Key == "last_name")?.Value));
            AddRoleClaims(claims, userMetadata.SingleOrDefault(x => x.Key == "wp_capabilities")?.Value);

            return new ClaimsPrincipal(new ClaimsIdentity(claims, "WordPress"));
        }

        private void VeryifyCookieHash(string cookieHash, long expiration, string username, string password, string scheme)
        {
            //if (string.IsNullOrWhiteSpace(password) || password.Length < 12)
            //    throw new InvalidOperationException("Expected password hash to be at least 12 characters");
            //string toHashString = $"{username}{password.Substring(8, 4)}|{expiration}";
            //string key = WpHash(toHashString, scheme);
            //string hash = HashHmacMd5(toHashString, )

            throw new NotImplementedException();

        }

        private void AddRoleClaims(ICollection<Claim> claims, string wpCapabilities)
        {
            if (!string.IsNullOrWhiteSpace(wpCapabilities))
            {
                if (!wpCapabilities.StartsWith("a:"))
                    throw new InvalidOperationException($"Unexpected string for wp_capabilities, expected it to start with 'a:' but got { wpCapabilities }");
                if (wpCapabilities.Length < 3)
                    throw new InvalidOperationException("Expected wp_capabilities to have at least 3 characters, but it only contained 'a:'");
                var indexOfNextColon = wpCapabilities.IndexOf(":", 3);
                if (indexOfNextColon < 3)
                    throw new InvalidOperationException("Unable to find second ':' in wp_capabilities (so couldn't determine how many roles the user has");
                if (!int.TryParse(wpCapabilities.Substring(2, indexOfNextColon - 2), out int numberOfRoles))
                    throw new InvalidOperationException($"Unable to determine how may roles the user has because the text in a:'#' was not a number. wp_capabilities = '{ wpCapabilities }'");

                // we expact the text after indexOfNextColon to be like {s:#:"roleName":b:1:}, and the text withing the brackets repeats x the number of roles
                int startIndex = wpCapabilities.IndexOf("{");
                int endIndex = wpCapabilities.LastIndexOf("}");
                if (startIndex < 2 || endIndex < 2 || endIndex <= startIndex)
                    throw new InvalidOperationException("Invalid wp_capabilities string. Expected string like 'a:1:{s:13:\"administrator\";b:1;}' but got '" + wpCapabilities + "'");

                foreach (var roleString in wpCapabilities.Substring(startIndex + 1, endIndex - startIndex - 1).Split(";b:1;"))
                {
                    string[] roleTokens = roleString.Split(":");
                    if (roleTokens.Length < 3)
                        continue; // unexpected...just continue on....
                    string roleName = roleTokens[2];
                    if (!string.IsNullOrWhiteSpace(roleName))
                        claims.Add(new Claim(ClaimTypes.Role, roleName.Replace("\"", string.Empty)));
                }
            }
        }
        #endregion


        #region Authentication to WordPress (Outgoing)
        public void SetWordPressCookies(CookieContainer cookieContainer)
        {
            Uri wordPressUrl = new Uri(_options.Url);
            var existingCookies = _httpContextAccessor.HttpContext?.Request?.Cookies;
            if (existingCookies != null && existingCookies.Any())
            {
                foreach (var c in existingCookies)
                {
                    cookieContainer.Add(wordPressUrl, new Cookie(c.Key, c.Value));
                }
            }
            else
            {
                string wordPressAuthCookieName = WordPressCookieAuthenticationHandler.GetWordPressCookieName(wordPressUrl.ToString());
                var existingCookie = _httpContextAccessor.HttpContext?.Request?.Cookies?.LastOrDefault(x => x.Key == wordPressAuthCookieName);
                if (existingCookie.HasValue && !string.IsNullOrWhiteSpace(existingCookie.Value.Value))
                {
                    cookieContainer.Add(wordPressUrl, new Cookie(existingCookie.Value.Key, existingCookie.Value.Value));
                }
                else
                {
                    _logger.Information("Unable to find authorization cookie in current HTTP context, so creating one from current user");
                    cookieContainer.Add(wordPressUrl, CreateWordPressCookie(_httpContextAccessor.HttpContext?.User));
                }
            }
        }

        public void SetWordPressNonce(HttpClient httpClient)
        {
            httpClient.DefaultRequestHeaders.Add("X-WP-Nonce", CreateNonce());
        }

        private string CreateNonce()
        {
            string action = "wp_rest"; // for our purposes it's always "wp-rest".
            var id = int.Parse(_httpContextAccessor.HttpContext.User.Claims.SingleOrDefault(x => x.Type == CLAIM_TYPE_ID).Value);

            // equivalent to wp_create_nonce: lines 2041-2052 in wp-includes/pluggable.php
            //var user = GetCurrentUser();
            var token = GetWordPressSessionToken(); // wp_get_session_token();
            var i = TickWordPressNone(); // wp_nonce_tick()

            string s = $"{i}|{action}|{id}|{token}";
            var hashed = WpHash(s, Scheme.NONCE);

            return hashed.Substring(hashed.Length - 12, 10);
        }

        private object TickWordPressNone()
        {
            // equivalent to wp_nonce_tick: lines 1950 - 1961 in wp-includes/pluggable.php
            double nonceLife = new TimeSpan(24, 0, 0).TotalSeconds;

            DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            double currentTime = DateTime.Now.ToUniversalTime().Subtract(epoch).TotalSeconds;

            return Math.Ceiling(currentTime / (nonceLife / 2));
        }

        private string GetWordPressSessionToken()
        {
            // equivalent to wp_get_session_token: lines 2454 - 2457 in wp-includes/user.php

            var cookie = ParseAuthCookie("", Scheme.LOGGED_IN);
            return cookie.Token;

        }

        private AuthCookie ParseAuthCookie(string cookie = "", Scheme? scheme = null)
        {
            // equivalent to wp_parse_auth_cookie: lines 752 - 787 in pluggable.php

            if (string.IsNullOrWhiteSpace(cookie) && _httpContextAccessor != null)
            {
                string cookieName;
                if (scheme.HasValue)
                {
                    switch(scheme.Value)
                    {
                        case Scheme.AUTH:
                            cookieName = "wordpress_";
                            break;
                        case Scheme.SECURE_AUTH:
                            cookieName = "wordpress_sec_";
                            break;
                        case Scheme.LOGGED_IN:
                            cookieName = "wordpress_logged_in_";
                            break;
                        default:
                            if (_options.IS_SSL)
                            {
                                cookieName = "wordpress_sec_";
                                scheme = Scheme.SECURE_AUTH;
                            }
                            else
                            {
                                cookieName = "wordpress_";
                                scheme = Scheme.AUTH;
                            }
                            break;
                    }
                }
                else
                {
                    if (_options.IS_SSL)
                    {
                        cookieName = "wordpress_sec_";
                        scheme = Scheme.SECURE_AUTH;
                    }
                    else
                    {
                        cookieName = "wordpress_";
                        scheme = Scheme.AUTH;
                    }
                }

                // get cookie based on scheme like the php code...
                var possibleCookies = _httpContextAccessor.HttpContext.Request.Cookies.Where(x => x.Key.StartsWith(cookieName)).ToList();
                if (possibleCookies.Count() == 1)
                    cookie = possibleCookies.Single().Value;
                else
                    throw new InvalidOperationException("More than one wordpress_logged_in cookie found, currently code not not support more than 1");
            }

            var tokens = cookie.Split("|");
            var returnValue = new AuthCookie();
            if (tokens.Length > 1)
                returnValue.UserName = tokens[0];
            if  (tokens.Length > 2 && long.TryParse(tokens[1], out long expiration))
            {
                DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
                returnValue.Expiration = epoch.AddSeconds(expiration);
            }
            if (tokens.Length > 3)
                returnValue.Token = tokens[2];
            if (tokens.Length > 4)
                returnValue.HMAC = tokens[3];
            return returnValue;
        }

        // equivalent to PHP's hash_hmac
        private string HashHmacMd5(string message, string salt)
        {
            var keyBytes = Encoding.UTF8.GetBytes(salt);
            using (var algorithm = new HMACMD5(keyBytes))
            {
                var hash = algorithm.ComputeHash(Encoding.UTF8.GetBytes(message));
                return BitConverter.ToString(hash).ToLower().Replace("-", string.Empty);
            }
        }

        private string WpHash(string message, Scheme scheme = Scheme.AUTH)
        {
            string salt = WpSalt(scheme);

            return HashHmacMd5(message, salt);
        }

        private static IDictionary<Scheme, string> cachedSalts = new Dictionary<Scheme, string>();

        private string WpSalt(Scheme scheme = Scheme.AUTH)
        {
            // see "pluggable.php", lines 2092 - 2161

            if (cachedSalts.ContainsKey(scheme))
                return cachedSalts[scheme];

            // not worrying about "Duplicated_Keys" for now

            string key = string.Empty, salt = string.Empty;
            if (!string.IsNullOrWhiteSpace(_options.SECRET_KEY))
                key = _options.SECRET_KEY;
            if (scheme == Scheme.AUTH && !string.IsNullOrWhiteSpace(_options.SECRET_SALT))
                salt = _options.SECRET_SALT;

            var optionProps = typeof(WordPressUserSecurityOptions).GetProperties();
            if (new Scheme[] { Scheme.AUTH, Scheme.SECURE_AUTH, Scheme.LOGGED_IN, Scheme.NONCE }.Contains(scheme))
            {
                key = optionProps.Single(x => x.Name == $"{scheme}_KEY").GetValue(_options) as string;
                salt = optionProps.Single(x => x.Name == $"{scheme}_SALT").GetValue(_options) as string;
            }
            else
            {
                // Scheme = SECRET
                throw new NotSupportedException("SECRET keys are not supported");
                //if (string.IsNullOrWhiteSpace(values["key"]))
                //{
                    //values["key"] = get_site_option("secret_key");
                    //if (! values["key"] )
                    //{
                    //    values["key"] = wp_generate_password(64, true, true);
                    //    update_site_option("secret_key", values["key"]);
                    //}
                //}
                //values["salt"] = hash_hmac("md5", scheme, values["key"]);
            }

            cachedSalts[scheme] = string.Concat(key, salt);

            return cachedSalts[scheme];
        }


        internal static Cookie CreateWordPressCookie(ClaimsPrincipal principal)
        {
            throw new NotImplementedException();

            //// create the cookie

            //// cookie has form:  Name|Expiration|Hash;


            //// TODO: generate a proper auth token
            //DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            //TimeSpan span = DateTime.Now.AddMinutes(10).Subtract(epoch);
            //long expiration = Convert.ToInt64(span.TotalSeconds);
            //existingCookie = new KeyValuePair<string, string>(wordPressAuthCookieName, I);


            //_logger.Warning("Unable to set authorization cookie as one was not able to be retrieved from HTTP context nor was one able to be create from current user");


        }
        #endregion



        private class AuthCookie
        {
            public string UserName { get; set; }
            public DateTime Expiration { get; set; }
            public string Token { get; set; }
            public string HMAC { get; set; }
        }


        internal enum Scheme
        {
            AUTH,
            SECURE_AUTH,
            LOGGED_IN,
            NONCE,
            SECRET

        }

    }
}

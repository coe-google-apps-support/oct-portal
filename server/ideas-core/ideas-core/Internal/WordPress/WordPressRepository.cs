using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CoE.Ideas.Core.Internal.WordPress
{
    internal class WordPressRepository : IWordPressRepository
    {
        public WordPressRepository(
            WordPressContext wordPressContext,
            Serilog.ILogger logger)
        {
            _wordPressContext = wordPressContext ?? throw new ArgumentNullException("wordPressContext");
            _logger = logger ?? throw new ArgumentNullException("logger");
        }

        private readonly WordPressContext _wordPressContext;
        private readonly Serilog.ILogger _logger;


        public async Task<ClaimsPrincipal> AuthenticateUserAsync(string cookie, string scheme = "auth")
        {
            if (string.IsNullOrWhiteSpace(cookie))
                throw new ArgumentNullException("cookie");

            _logger.Information("Attempting to authenticate user using WordPress");

            string[] cookieParts = cookie.Split("|");
            if (cookieParts.Length < 3)
            {
                _logger.Error("Expected cookie to contain Name|Expiration|Hash: { WordPressCookie }", cookie);
                throw new InvalidOperationException($"Expected cookie to contain Name|Expiration|Hash, but only got { cookieParts.Length } parts to the cookie");
            }
            else
            {
                if (!long.TryParse(cookieParts[1], out long expiration))
                {
                    _logger.Error("Unable to determine expiration of WordPress cookie as it was not a 64-bit integer: { WordPressCookie }", cookie);
                    throw new InvalidOperationException("Unable to determine expiration of WordPress cookie as it was not a 64-bit integer");
                }
                else
                {
                    DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
                    if (epoch.AddSeconds(expiration) <= DateTime.Now)
                    {
                        _logger.Error("WordPress cookie is expired: { WordPressCookie }", cookie);
                        throw new InvalidOperationException("WordPress cookie is expired");
                    }
                    else
                    {
                        string username = System.Web.HttpUtility.UrlDecode(cookieParts[0]);
                        return await VerifyHashAndCreatePrincipal(username, expiration, cookieHash: cookieParts[2], scheme: scheme);
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
                .Where(x => metaKeys.Contains(x.Key))
                .Select(x => new { x.Key, x.Value })
                .ToListAsync();

            // TODO: implement cookie hash verification like WordPress
            //VeryifyCookieHash(cookieHash, expiration, username, userInfo.Password, scheme);

            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, username),
                new Claim(ClaimTypes.Email, userInfo.Email),
                new Claim(ClaimTypes.Name, userInfo.Name),
                new Claim(ClaimTypes.Uri, userInfo.Url)
            };

            // augment with user metadata
            var userMetadata = await metadataInfoTask;
            claims.Add(new Claim(ClaimTypes.GivenName, userMetadata.FirstOrDefault(x => x.Key == "first_name")?.Value));
            claims.Add(new Claim(ClaimTypes.Surname, userMetadata.FirstOrDefault(x => x.Key == "last_name")?.Value));
            AddRoleClaims(claims, userMetadata.SingleOrDefault(x => x.Key == "wp_capabilities")?.Value);

            return new ClaimsPrincipal(new ClaimsIdentity(claims, "WordPress"));
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
                if (!int.TryParse(wpCapabilities.Substring(2, indexOfNextColon-2), out int numberOfRoles))
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

        private void VeryifyCookieHash(string cookieHash, long expiration, string username, string password, string scheme)
        {
            //if (string.IsNullOrWhiteSpace(password) || password.Length < 12)
            //    throw new InvalidOperationException("Expected password hash to be at least 12 characters");
            //string toHashString = $"{username}{password.Substring(8, 4)}|{expiration}";
            //string key = WpHash(toHashString, scheme);
            //string hash = HashHmacMd5(toHashString, )

            throw new NotImplementedException();

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

        private string WpHash(string message)
        {
            string salt = WpSalt(message);

            return HashHmacMd5(message, salt);
        }

        private string WpSalt(string s)
        {
            // see "pluggable.php", lines 2092 - 2161
            throw new NotImplementedException();
        }
    }
}

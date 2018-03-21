using CoE.Ideas.Shared.Security.Events;
using CoE.Ideas.Shared.WordPress;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace CoE.Ideas.Shared.Security
{
    internal class WordPressCookieAuthenticationHandler : AuthenticationHandler<WordPressCookieAuthenticationOptions>
    {
        public WordPressCookieAuthenticationHandler(IOptionsMonitor<WordPressCookieAuthenticationOptions> options,
            IWordPressUserSecurity wordPressUserSecurity,
            ILoggerFactory logger,
            UrlEncoder encoder,
            //IDataProtectionProvider dataProtection, 
            ISystemClock clock) : base(options, logger, encoder, clock)
        {
            _wordPressUserSecurity = wordPressUserSecurity ?? throw new ArgumentNullException("wordPressUserSecurity");

            if (logger == null)
                throw new ArgumentNullException("logger");
            _logger = logger.CreateLogger<WordPressCookieAuthenticationHandler>() ?? throw new ArgumentNullException("logger");
        }

        private readonly IWordPressUserSecurity _wordPressUserSecurity;
        private readonly ILogger<WordPressCookieAuthenticationHandler> _logger;

        /// <summary>
        /// The handler calls methods on the events which give the application control at certain points where processing is occurring. 
        /// If it is not provided a default instance is supplied which does nothing when the methods are called.
        /// </summary>
        protected new WordPressCookieEvents Events
        {
            get { return (WordPressCookieEvents)base.Events; }
            set { base.Events = value; }
        }

        protected override Task<object> CreateEventsAsync() => Task.FromResult<object>(new WordPressCookieEvents());

        /// <summary>
        /// Looks for the WordPress_cookie_ and verifies the cookie
        /// </summary>
        /// <returns></returns>
        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var context = new CookieReceivedContext(Context, Scheme, Options);


            if (Events.OnValidateCookie == null)
            {
                await OnValidateCookieDefaultHandler(context);
            }
            else
            {
                _logger.LogInformation("Validating WordPress cookie using custom event");
                await Events.OnValidateCookie(context);
            }

#if DEBUG
            if (context.Principal == null || !context.Principal.Identity.IsAuthenticated)
            {
                if (!string.IsNullOrWhiteSpace(Options.DevUserEmail) || !string.IsNullOrWhiteSpace(Options.DevUserName))
                {
                    context.Principal = WordPressUserSecurity.CreateDevUser(Options.DevUserName, Options.DevUserEmail);
                    return AuthenticateResult.Success(new AuthenticationTicket(context.Principal, context.Properties, Scheme.Name));
                }
            }
#endif



            if (context.Principal == null)
                return AuthenticateResult.Fail("No principal.");

            return AuthenticateResult.Success(new AuthenticationTicket(context.Principal, context.Properties, Scheme.Name));
        }

        protected virtual async Task OnValidateCookieDefaultHandler(CookieReceivedContext context)
        {
            _logger.LogInformation("Validating WordPress cookie using default event and WordPress url {WordPressUrl}", Options.WordPressUrl);
            var cookie = context.HttpContext.Request.Cookies[GetWordPressCookieName(wordPressUrl: Options.WordPressUrl)];
            if (string.IsNullOrWhiteSpace(cookie))
            {
                _logger.LogError("WordPress cookie not found for url {WordPressUrl}", Options.WordPressUrl);
#if !DEBUG
                context.Fail($"WordPress cookie not found for url { Options.WordPressUrl }");
#endif
            }
            else
            {
                _logger.LogDebug("WordPress cookie found with value {WordPressCookie}", cookie);
                try
                {
                    context.Principal = await _wordPressUserSecurity.AuthenticateUserAsync(cookie);
                    if (context.Principal.Identity.IsAuthenticated)
                        _logger.LogInformation("Authenticated user {UserName}", context.Principal.Identity.Name);
                    else
                    {
                        _logger.LogWarning("WordPressCookie reported success however identity was not authenticated. Cookie: { WordPressCookie }", cookie);
                    }
                }
                catch (Exception err)
                {
                    _logger.LogError(err, "Unable to create Principal from cookie '{WordPressCookie}'; {ErrorMessage}", cookie, err.Message);
#if !DEBUG
                    context.Fail(err);
#endif
                }
            }
        }


        private static IDictionary<string, string> wordPressCookieNameMap = new Dictionary<string, string>();
        internal static string GetWordPressCookieName(string wordPressUrl)
        {
            // simple caching :)
            if (wordPressCookieNameMap.ContainsKey(wordPressUrl))
                return wordPressCookieNameMap[wordPressUrl];

            string wordPressUrlHash;
            using (var md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] data = md5.ComputeHash(Encoding.UTF8.GetBytes(wordPressUrl));
                // Create a new Stringbuilder to collect the bytes
                // and create a string.
                StringBuilder sBuilder = new StringBuilder();

                // Loop through each byte of the hashed data 
                // and format each one as a hexadecimal string.
                for (int i = 0; i < data.Length; i++)
                {
                    sBuilder.Append(data[i].ToString("x2"));
                }

                // Return the hexadecimal string.
                wordPressUrlHash = sBuilder.ToString();
            }

            string returnValue = $"wordpress_logged_in_{wordPressUrlHash}";
            wordPressCookieNameMap[wordPressUrl] = returnValue;
            return returnValue;
        }
    }
}

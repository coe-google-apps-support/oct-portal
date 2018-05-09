using CoE.Ideas.Shared.Extensions;
using CoE.Ideas.Shared.Security.Events;
using CoE.Ideas.Shared.WordPress;
using EnsureThat;
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
            ILoggerFactory loggerFactory,
            Serilog.ILogger logger,
            UrlEncoder encoder,
            ISystemClock clock) : base(options, loggerFactory, encoder, clock)
        {
            EnsureArg.IsNotNull(wordPressUserSecurity);
            EnsureArg.IsNotNull(logger);
            _wordPressUserSecurity = wordPressUserSecurity;
            _logger = logger;
        }

        private readonly IWordPressUserSecurity _wordPressUserSecurity;
        private readonly Serilog.ILogger _logger;

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

            // SPN take precedence
            var headers = context.HttpContext?.Request?.Headers;
            if (headers != null)
            {
                Microsoft.Extensions.Primitives.StringValues authHeader;
                if (headers.TryGetValue("Authorization", out authHeader))
                {
                    if (authHeader.Count > 0 && authHeader[0].StartsWith("SPN ") && authHeader[0].Length > 4)
                    {
                        context.Principal = _wordPressUserSecurity.TryCreateServicePrincipal(authHeader[0].Substring(4));
                    }
                }
            }

            if (context.Principal == null || !context.Principal.Identity.IsAuthenticated)
            {
                if (Events.OnValidateCookie == null)
                {
                    await OnValidateCookieDefaultHandler(context);
                }
                else
                {
                    _logger.Information("Validating WordPress cookie using custom event");
                    await Events.OnValidateCookie(context);
                }
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

        // this whole method should be moved to WordPressUserSecurity -DC

        protected virtual async Task OnValidateCookieDefaultHandler(CookieReceivedContext context)
        {
            _logger.Information("Validating WordPress cookie using default event and WordPress url {WordPressUrl}", Options.WordPressUrl);
            var cookie = context.HttpContext.Request.Cookies[GetWordPressCookieName(wordPressUrl: Options.WordPressUrl)];
            if (string.IsNullOrWhiteSpace(cookie))
            {
                _logger.Error("WordPress cookie not found for url {WordPressUrl}", Options.WordPressUrl);
#if !DEBUG
                context.Fail($"WordPress cookie not found for url { Options.WordPressUrl }");
#endif
            }
            else
            {
                _logger.Debug("WordPress cookie found with value {WordPressCookie}", cookie);
                try
                {
                    context.Principal = await _wordPressUserSecurity.AuthenticateUserAsync(cookie);
                    if (context.Principal.Identity.IsAuthenticated)
                        _logger.Information("Authenticated user {UserName}", context.Principal.Identity.Name);
                    else
                    {
                        _logger.Warning("WordPressCookie reported success however identity was not authenticated. Cookie: { WordPressCookie }", cookie);
                    }
                }
                catch (Exception err)
                {
                    _logger.Error(err, "Unable to create Principal from cookie '{WordPressCookie}'; {ErrorMessage}", cookie, err.Message);
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

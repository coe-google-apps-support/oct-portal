using CoE.Ideas.Core.Internal.WordPress;
using CoE.Ideas.Core.Security.Events;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace CoE.Ideas.Core.Security
{
    internal class WordPressCookieAuthenticationHandler : AuthenticationHandler<WordPressCookieAuthenticationOptions>
    {
        public WordPressCookieAuthenticationHandler(IOptionsMonitor<WordPressCookieAuthenticationOptions> options,
            IWordPressRepository wordPressRepository,
            ILoggerFactory logger, 
            UrlEncoder encoder, 
            //IDataProtectionProvider dataProtection, 
            ISystemClock clock) : base(options, logger, encoder, clock)
        {
            _wordPressRepository = wordPressRepository ?? throw new ArgumentNullException("wordPressRepository");
        }

        private readonly IWordPressRepository _wordPressRepository;

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
                await Events.OnValidateCookie(context);

            if (context.Principal == null)
                return AuthenticateResult.Fail("No principal.");

            return AuthenticateResult.Success(new AuthenticationTicket(context.Principal, context.Properties, Scheme.Name));
        }

        protected virtual async Task OnValidateCookieDefaultHandler(CookieReceivedContext context)
        {
            var cookie = context.HttpContext.Request.Cookies[GetWordPressCookieName(wordPressUrl: Options.WordPressUrl)];
            if (string.IsNullOrWhiteSpace(cookie))
                context.Fail($"WordPress cookie not found for url { Options.WordPressUrl }");
            else
            {
                try
                {
                    context.Principal = await _wordPressRepository.AuthenticateUserAsync(cookie);
                }
                catch (Exception err)
                {
                    context.Fail(err);
                }
            }
        }


        private static IDictionary<string, string> wordPressCookieNameMap = new Dictionary<string, string>();
        private static string GetWordPressCookieName(string wordPressUrl)
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

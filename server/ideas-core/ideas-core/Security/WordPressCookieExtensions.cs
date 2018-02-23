using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoE.Ideas.Core.Security
{
    public static class WordPressCookieExtensions
    {
        public static AuthenticationBuilder AddWordPressCookie(this AuthenticationBuilder builder)
            => builder.AddWordPressCookie(WordPressCookieAuthenticationDefaults.AuthenticationScheme);

        public static AuthenticationBuilder AddWordPressCookie(this AuthenticationBuilder builder, string authenticationScheme)
            => builder.AddWordPressCookie(authenticationScheme, configureOptions: null);

        public static AuthenticationBuilder AddWordPressCookie(this AuthenticationBuilder builder, Action<WordPressCookieAuthenticationOptions> configureOptions)
            => builder.AddWordPressCookie(WordPressCookieAuthenticationDefaults.AuthenticationScheme, configureOptions);

        public static AuthenticationBuilder AddWordPressCookie(this AuthenticationBuilder builder, string authenticationScheme, Action<WordPressCookieAuthenticationOptions> configureOptions)
            => builder.AddWordPressCookie(authenticationScheme, displayName: null, configureOptions: configureOptions);

        public static AuthenticationBuilder AddWordPressCookie(this AuthenticationBuilder builder, string authenticationScheme, string displayName, Action<WordPressCookieAuthenticationOptions> configureOptions)
        {
            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IPostConfigureOptions<WordPressCookieAuthenticationOptions>, PostConfigureWordPressCookieAuthenticationOptions>());
            return builder.AddScheme<WordPressCookieAuthenticationOptions, WordPressCookieAuthenticationHandler>(authenticationScheme, displayName, configureOptions);
        }
    }
}

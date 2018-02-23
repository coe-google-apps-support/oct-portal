using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoE.Ideas.Core.Security
{
    public class PostConfigureWordPressCookieAuthenticationOptions : IPostConfigureOptions<WordPressCookieAuthenticationOptions>
    {
        //private readonly IDataProtectionProvider _dp;

        //public PostConfigureWordPressCookieAuthenticationOptions(IDataProtectionProvider dataProtection)
        //{
        //    _dp = dataProtection;
        //}

        public void PostConfigure(string name, WordPressCookieAuthenticationOptions options)
        {
            //options.DataProtectionProvider = options.DataProtectionProvider ?? _dp;


            //// see https://github.com/aspnet/Security/blob/dev/src/Microsoft.AspNetCore.Authentication.JwtBearer/JwtBearerPostConfigureOptions.cs
            //// for example

            //throw new NotImplementedException();
        }
    }
}

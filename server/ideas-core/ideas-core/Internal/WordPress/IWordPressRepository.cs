using CoE.Ideas.Core.WordPress;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CoE.Ideas.Core.Internal.WordPress
{
    internal interface IWordPressRepository
    {
        Task<ClaimsPrincipal> AuthenticateUserAsync(string cookie, string scheme = "auth");
    }
}

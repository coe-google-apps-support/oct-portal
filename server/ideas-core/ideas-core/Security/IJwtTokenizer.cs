using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace CoE.Ideas.Core.Security
{
    public interface IJwtTokenizer
    {
        string CreateJwt(ClaimsPrincipal principal);
        ClaimsPrincipal CreatePrincipal(string jwt);
    }
}

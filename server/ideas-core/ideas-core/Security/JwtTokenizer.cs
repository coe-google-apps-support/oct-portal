using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace CoE.Ideas.Core.Security
{
    internal class JwtTokenizer : IJwtTokenizer
    {
        public string CreateJwt(ClaimsPrincipal principal)
        {
            throw new NotImplementedException();
        }

        public ClaimsPrincipal CreatePrincipal(string jwt)
        {
            throw new NotImplementedException();
        }
    }
}

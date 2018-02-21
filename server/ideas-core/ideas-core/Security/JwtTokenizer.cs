using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;

namespace CoE.Ideas.Core.Security
{
    internal class JwtTokenizer : IJwtTokenizer
    {
        public JwtTokenizer(IOptions<JwtTokenizerOptions> options)
        {
            _options = options.Value;

            if (!string.IsNullOrWhiteSpace(options?.Value?.JwtSecretKey))
                _signingKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(Encoding.ASCII.GetBytes(options.Value.JwtSecretKey));
        }

        private readonly Microsoft.IdentityModel.Tokens.SymmetricSecurityKey _signingKey;
        private readonly JwtTokenizerOptions _options;

        public string CreateJwt(ClaimsPrincipal principal)
        {
            // Create JWT credentials here
            var now = DateTime.Now;
            var jwt = new JwtSecurityToken(
                issuer: _options.WordPressUrl.ToString(),
                notBefore: now,
                expires: now.AddMinutes(5),
                claims: principal.Claims,
                signingCredentials: new Microsoft.IdentityModel.Tokens.SigningCredentials(
                    _signingKey,
                    Microsoft.IdentityModel.Tokens.SecurityAlgorithms.HmacSha256));

            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }

        public ClaimsPrincipal CreatePrincipal(string jwtToken)
        {
            var jwt = new JwtSecurityToken(jwtToken);

            var nameClaim = jwt.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name);
            if (string.IsNullOrWhiteSpace(nameClaim?.Value))
                throw new InvalidOperationException("Unable to get name of user");

            return new ClaimsPrincipal(new ClaimsIdentity(new GenericIdentity(nameClaim.Value), jwt.Claims));
        }
    }
}

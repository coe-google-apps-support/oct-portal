using System;
using System.Linq;
using System.Security.Claims;

namespace CoE.Ideas.Shared.Security
{
    public static class ClaimsPrincipalExtensions
    {
        public static string GetEmail(this ClaimsPrincipal principal)
        {
            var emailClaim = principal.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email);
            if (emailClaim == null)
                throw new InvalidOperationException("Unable to find a claim of type EMmail");
            else
                return emailClaim.Value;
        }

        public static string GetDisplayName(this ClaimsPrincipal principal)
        {
            var displayNameClaim = principal.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name);
            if (displayNameClaim == null)
                throw new InvalidOperationException("Unable to find a claim of type Name");
            else
                return displayNameClaim.Value;
        }
    }

}

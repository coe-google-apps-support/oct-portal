using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace CoE.Ideas.Shared.Security
{
    internal class PermissionsClaimPrincipal : ClaimsPrincipal, IPermissionsClaimPrincipal
    {
        public PermissionsClaimPrincipal(ClaimsPrincipal basePrincipal,
            SecurityContext securityContext) : base(basePrincipal)
        {
            _basePrincipal = basePrincipal;
            _securityContext = securityContext;
        }

        private readonly SecurityContext _securityContext;
        private readonly ClaimsPrincipal _basePrincipal;

        public override IEnumerable<Claim> Claims => _basePrincipal.Claims;

        public bool HasPermission(string permissionName)
        {
            var roleNames = Claims.Where(x => x.Type == ClaimTypes.Role)
                .Select(x => x.Value)
                .ToArray();

            return _securityContext.Permissions.Where(x => roleNames.Contains(x.Role))
                .Any(x => x.Permssion == permissionName);
        }
    }
}

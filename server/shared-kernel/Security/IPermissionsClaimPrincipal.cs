using System;
using System.Collections.Generic;
using System.Text;

namespace CoE.Ideas.Shared.Security
{
    public interface IPermissionsClaimPrincipal
    {
        bool HasPermission(string permissionName);
    }
}

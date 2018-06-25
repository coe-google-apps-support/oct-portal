using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace CoE.Ideas.Shared.Security
{
    public interface ICurrentUserAccessor
    {
        ClaimsPrincipal User { get;  }
    }
}

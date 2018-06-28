using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace CoE.Ideas.Core.Services
{
    public interface IRemoteRepository
    {
        void SetUser(ClaimsPrincipal user);
    }
}

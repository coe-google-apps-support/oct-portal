using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Claims;


namespace CoE.Issues.Core.Services
{
    public interface IRemoteRepository
    {
        void SetUser(ClaimsPrincipal user);
    }
}

using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace CoE.Ideas.Core
{
    public interface IIdeaRepositoryFactory
    {
        IIdeaRepository Create(ClaimsPrincipal user);
    }
}

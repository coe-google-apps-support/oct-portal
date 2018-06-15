using EnsureThat;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace CoE.Ideas.Shared.Security
{
    internal class HttpContextCurrentUserAccessor : ICurrentUserAccessor
    {
        public HttpContextCurrentUserAccessor(IHttpContextAccessor httpContextAccessor)
        {
            EnsureArg.IsNotNull(httpContextAccessor);
            _httpContextAccessor = httpContextAccessor;
        }

        private readonly IHttpContextAccessor _httpContextAccessor;

        public ClaimsPrincipal User => _httpContextAccessor.HttpContext?.User;
    }
}

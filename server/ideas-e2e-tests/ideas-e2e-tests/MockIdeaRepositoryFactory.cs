using System.Security.Claims;
using CoE.Ideas.Core;
using CoE.Ideas.Core.Tests;

namespace CoE.Ideas.EndToEnd.Tests
{
    internal class MockIdeaRepositoryFactory : IIdeaRepositoryFactory
    {
        public IIdeaRepository Create(ClaimsPrincipal user)
        {
            return new MockIdeaRepository();
        }
    }
}
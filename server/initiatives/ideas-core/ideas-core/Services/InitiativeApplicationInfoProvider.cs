using EnsureThat;
using System;


namespace CoE.Ideas.Core.Services
{
    internal class InitiativeApplicationInfoProvider : IInitiativeApplicationInfoProvider
    {
        public InitiativeApplicationInfoProvider(string applicationUrl)
        {
            EnsureArg.IsNotNullOrWhiteSpace(applicationUrl);

            _applicationUrl = new Uri(applicationUrl);
        }
        private readonly Uri _applicationUrl;

        public Uri GetApplicationUrl()
        {
            return _applicationUrl;
        }
    }
}

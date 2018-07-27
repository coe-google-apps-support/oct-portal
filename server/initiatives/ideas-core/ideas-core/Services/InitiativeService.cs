using System;
using System.Collections.Generic;
using System.Text;
using CoE.Ideas.Core.Data;
using EnsureThat;

namespace CoE.Ideas.Core.Services
{
    internal class InitiativeService : IInitiativeService
    {
        public InitiativeService(IInitiativeApplicationInfoProvider initiativeApplicationInfoProvider)
        {
            EnsureArg.IsNotNull(initiativeApplicationInfoProvider);
            _initiativeApplicationInfoProvider = initiativeApplicationInfoProvider;
        }

        private readonly IInitiativeApplicationInfoProvider _initiativeApplicationInfoProvider;

        public Uri GetInitiativeUrl(int initiativeId)
        {
            return new Uri(_initiativeApplicationInfoProvider.GetApplicationUrl(),
                "/view-ideas/?id=" + initiativeId);
        }
    }
}
